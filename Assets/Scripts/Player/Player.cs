using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// grid-step movement with a little slide between cells
// зажал клавишу — шагаем дальше; ЛКМ по клетке — автопуть через A*
public class Player : MonoBehaviour
{
    // где мы сейчас на сетке
    public int gridX;
    public int gridY;

    // ссылка на менеджер, даёт нам мир-координаты и размеры
    public GridManager grid;

    // сколько клеток в секунду проезжаем, крутить в инспекторе
    public float moveSpeed = 6f;

    [Header("Walk Animation")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float walkFrameSeconds = 0.16f;
    [SerializeField] Sprite[] frontWalkSprites;
    [SerializeField] Sprite[] backWalkSprites;
    [SerializeField] Sprite[] leftWalkSprites;
    [SerializeField] Sprite[] rightWalkSprites;

    // состояние скольжения между клетками
    bool isMoving;
    Vector3 moveFrom;
    Vector3 moveTarget;
    float moveT;

    enum FacingDirection
    {
        Front,
        Back,
        Left,
        Right
    }

    FacingDirection facingDirection = FacingDirection.Front;
    float walkAnimationTime;
    bool walkAnimationActive;

    // очередь клеток от A* — по одной за слайд
    List<Vector2Int> path;
    int pathIndex;

    // PCG: level completion — ставится в true когда все монетки собраны
    // пока true — не слушаем никакой ввод (клавиши, мышь, шаги A*)
    public bool inputLocked;

    public bool IsMoving => isMoving;
    public bool HasQueuedPath => path != null && pathIndex < path.Count;
    public bool IsBusy => isMoving || HasQueuedPath;

    // кэшируем камеру один раз, чтоб не дёргать Camera.main каждый клик
    Camera cam;
    int ignoreClickToMoveFrame = -1;

    void Start()
    {
        cam = Camera.main;
        ResolveSpriteRenderer();
        SetIdleSprite();
    }

    void Update()
    {
        // если извне телепортнули (R regenerate) — слайд отменяем
        // иначе он дотянет нас до старой цели которой уже нет ( °-°)
        if (isMoving && grid != null)
        {
            Vector3 expected = grid.GridToWorld(gridX, gridY);
            if ((expected - moveTarget).sqrMagnitude > 0.0001f)
            {
                isMoving = false;
                transform.position = expected;
                SetIdleSprite();
            }
        }

        if (isMoving)
        {
            // плавно едем от одной клетки к другой
            moveT += Time.deltaTime * moveSpeed;
            if (moveT >= 1f)
            {
                transform.position = moveTarget; // ровно на клеточке стоим
                isMoving = false;
                if (ShouldKeepWalkPoseAfterStep())
                    RefreshWalkSprite();
                else
                    SetIdleSprite();
            }
            else
            {
                transform.position = Vector3.Lerp(moveFrom, moveTarget, moveT);
                RefreshWalkSprite();
            }
            return; // пока едем — ничего не слушаем, а то застрянем посерединке
        }

        // after Completed! — dno of input, тихо стоим пока LevelGoal не разблокирует
        if (inputLocked) return;

        // ЛКМ по клетке — прокладываем маршрут A*
        if (Input.GetMouseButtonDown(0) && cam != null && grid != null && !IsPointerOverUI())
            TryClickToMove();

        // GetKey, чтобы зажатие продолжало шагать без долбёжки по клавише
        // вертикаль первее, чтоб случайно не поехать по диагонали
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            StartStep(0, 1);
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            StartStep(0, -1);
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            StartStep(-1, 0);
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            StartStep(1, 0);

        // если клавиш нет и есть очередь из A* — делаем следующий шаг
        if (!isMoving && path != null && pathIndex < path.Count)
        {
            Vector2Int next = path[pathIndex];
            pathIndex++;
            StartStepTo(next.x, next.y);
            if (pathIndex >= path.Count) CancelPath(); // дошли, чистим
        }
    }

    // мышь -> клетка -> A* -> запомнить маршрут
    void TryClickToMove()
    {
        if (ignoreClickToMoveFrame == Time.frameCount) return;

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0f;

        if (!grid.TryWorldToGrid(world, out int tx, out int ty)) return;
        if (grid.HasActiveLootAt(tx, ty)) return;

        TryMoveToCell(tx, ty);
    }

    public bool TryMoveToCell(int targetX, int targetY)
    {
        if (grid == null) return false;

        List<Vector2Int> found = Pathfinder.FindPath(grid, gridX, gridY, targetX, targetY);
        if (found == null || found.Count <= 1) return false; // нет пути или та же клетка

        path = found;
        pathIndex = 1; // нулевой элемент — это мы сами, пропускаем
        return true;
    }

    public bool IsAdjacentToCell(int x, int y)
    {
        int dx = Mathf.Abs(x - gridX);
        int dy = Mathf.Abs(y - gridY);
        return dx + dy == 1;
    }

    public void IgnoreClickToMoveThisFrame()
    {
        ignoreClickToMoveFrame = Time.frameCount;
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    // манул-шаг с клавиатуры — он всегда перебивает автопуть
    void StartStep(int dx, int dy)
    {
        CancelPath();
        StartStepTo(gridX + dx, gridY + dy);
    }

    // общий старт слайда на конкретную соседнюю клетку
    // используется и клавишами, и очередью A*
    void StartStepTo(int nx, int ny)
    {
        // стенки карты, дальше нельзя
        if (nx < 0 || nx >= grid.width) return;
        if (ny < 0 || ny >= grid.height) return;

        // в лес не ходим, там волки ( ˘･_･˘ )
        if (grid.cells[nx, ny].type == CellType.Forest) return;
        if (grid.HasActiveLootAt(nx, ny)) return;

        SetFacingDirection(nx - gridX, ny - gridY);

        gridX = nx;
        gridY = ny;

        // туман разгоняем сразу при шаге, чтоб чувствовалось отзывчиво
        grid.RevealArea(gridX, gridY, grid.revealRadius);

        moveFrom = transform.position;
        moveTarget = grid.GridToWorld(gridX, gridY);
        moveT = 0f;
        if (!walkAnimationActive)
            walkAnimationTime = 0f;

        isMoving = true;
        RefreshWalkSprite();
    }

    // очистить очередь автопути — зовётся и самим игроком, и GridManager при R
    public void CancelPath()
    {
        path = null;
        pathIndex = 0;
    }

    // hard stop: cancel A* queue AND kill current slide
    // нужно чтобы после Completed! игрок не доезжал и не догонял старый путь
    public void ForceStop()
    {
        CancelPath();
        if (isMoving)
        {
            transform.position = moveTarget; // доезжаем ровно на клетку, чтоб не висеть между
            isMoving = false;
        }
        moveT = 0f;
        SetIdleSprite();
    }

    void ResolveSpriteRenderer()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void SetFacingDirection(int dx, int dy)
    {
        if (dx > 0)
            facingDirection = FacingDirection.Right;
        else if (dx < 0)
            facingDirection = FacingDirection.Left;
        else if (dy > 0)
            facingDirection = FacingDirection.Back;
        else if (dy < 0)
            facingDirection = FacingDirection.Front;
    }

    void RefreshWalkSprite()
    {
        if (spriteRenderer == null)
            return;

        Sprite[] frames = GetFramesForFacing();
        if (frames == null || frames.Length == 0)
            return;

        walkAnimationActive = true;
        walkAnimationTime += Time.deltaTime;
        int firstWalkFrame = frames.Length > 1 ? 1 : 0;
        int walkFrameCount = frames.Length - firstWalkFrame;
        int frameIndex = firstWalkFrame + Mathf.FloorToInt(walkAnimationTime / Mathf.Max(0.01f, walkFrameSeconds)) % walkFrameCount;
        if (frames[frameIndex] != null)
            spriteRenderer.sprite = frames[frameIndex];
    }

    void SetIdleSprite()
    {
        if (spriteRenderer == null)
            return;

        Sprite[] frames = GetFramesForFacing();
        if (frames == null || frames.Length == 0 || frames[0] == null)
            return;

        walkAnimationActive = false;
        walkAnimationTime = 0f;
        spriteRenderer.sprite = frames[0];
    }

    bool ShouldKeepWalkPoseAfterStep()
    {
        if (HasQueuedPath)
            return true;

        if (inputLocked)
            return false;

        return Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.UpArrow)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.LeftArrow)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.RightArrow);
    }

    Sprite[] GetFramesForFacing()
    {
        switch (facingDirection)
        {
            case FacingDirection.Back:
                return backWalkSprites;
            case FacingDirection.Left:
                return leftWalkSprites;
            case FacingDirection.Right:
                return rightWalkSprites;
            default:
                return frontWalkSprites;
        }
    }

    void OnValidate()
    {
        walkFrameSeconds = Mathf.Max(0.01f, walkFrameSeconds);
    }
}
