using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HerbPickupController : MonoBehaviour
{
    [SerializeField] GridManager grid;
    [SerializeField] HerbInventory inventory;
    [SerializeField] SimpleRewardPopup rewardPopup;
    [SerializeField] Camera worldCamera;
    [SerializeField] int pickupRadius = 1;
    [SerializeField] HerbType fallbackHerbType = HerbType.BellLeaf;

    static readonly Vector2Int[] CardinalDirections =
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };

    bool hasPendingPickup;
    Vector2Int pendingPickupCell;

    void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    void Update()
    {
        TryCompletePendingPickup();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (TryPickupNearPlayer())
                return;
        }

        HandleTapOrClickPickup();
    }

    bool TryPickupNearPlayer()
    {
        if (!CanTryPickup()) return false;

        int playerX = grid.player.gridX;
        int playerY = grid.player.gridY;
        int radius = Mathf.Clamp(pickupRadius, 0, 1);

        if (radius <= 0)
            return false;

        for (int i = 0; i < CardinalDirections.Length; i++)
        {
            Vector2Int direction = CardinalDirections[i];

            if (TryPickupAt(playerX + direction.x, playerY + direction.y))
                return true;
        }

        return false;
    }

    void HandleTapOrClickPickup()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended && !IsPointerOverUI(touch.fingerId))
                TryHandleHerbTapFromScreenPosition(touch.position);

            return;
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            TryHandleHerbTapFromScreenPosition(Input.mousePosition);
    }

    bool TryHandleHerbTapFromScreenPosition(Vector2 screenPosition)
    {
        if (!HasPickupReferences()) return false;
        if (worldCamera == null) return false;

        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        if (!grid.TryWorldToGrid(worldPosition, out int x, out int y))
            return false;

        if (!grid.HasActiveLootAt(x, y))
            return false;

        grid.player.IgnoreClickToMoveThisFrame();

        if (!CanTryPickup()) return true;

        if (IsCardinalAdjacentToHerb(x, y))
        {
            ClearPendingPickup();
            TryPickupAt(x, y);
            return true;
        }

        if (FindBestAdjacentCellToHerb(x, y, out Vector2Int moveTarget)
            && grid.player.TryMoveToCell(moveTarget.x, moveTarget.y))
        {
            pendingPickupCell = new Vector2Int(x, y);
            hasPendingPickup = true;
        }

        return true;
    }

    bool TryPickupAt(int x, int y)
    {
        if (grid == null || !grid.IsInsideGrid(x, y)) return false;

        if (!grid.TryPickupHerbAt(x, y, out HerbType pickedType))
            return false;

        if (!IsKnownHerbType(pickedType))
            pickedType = fallbackHerbType;

        inventory.AddHerb(pickedType);

        if (rewardPopup != null)
            rewardPopup.Show(GetHerbDisplayName(pickedType));

        return true;
    }

    bool HasPickupReferences()
    {
        return grid != null && grid.player != null && inventory != null;
    }

    bool CanTryPickup()
    {
        if (!HasPickupReferences()) return false;
        if (grid.player.inputLocked) return false;
        if (grid.player.IsBusy) return false;

        return true;
    }

    void TryCompletePendingPickup()
    {
        if (!hasPendingPickup) return;

        if (!HasPickupReferences())
        {
            ClearPendingPickup();
            return;
        }

        if (!grid.HasActiveLootAt(pendingPickupCell.x, pendingPickupCell.y))
        {
            ClearPendingPickup();
            return;
        }

        if (grid.player.inputLocked) return;
        if (grid.player.IsBusy) return;

        if (IsCardinalAdjacentToHerb(pendingPickupCell.x, pendingPickupCell.y))
        {
            TryPickupAt(pendingPickupCell.x, pendingPickupCell.y);
            ClearPendingPickup();
        }
    }

    bool IsCardinalAdjacentToHerb(int herbX, int herbY)
    {
        int dx = Mathf.Abs(grid.player.gridX - herbX);
        int dy = Mathf.Abs(grid.player.gridY - herbY);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    bool FindBestAdjacentCellToHerb(int herbX, int herbY, out Vector2Int moveTarget)
    {
        moveTarget = Vector2Int.zero;
        if (!HasPickupReferences()) return false;

        bool foundTarget = false;
        int bestPathLength = int.MaxValue;

        for (int i = 0; i < CardinalDirections.Length; i++)
        {
            Vector2Int direction = CardinalDirections[i];
            int x = herbX + direction.x;
            int y = herbY + direction.y;
            if (!grid.IsInsideGrid(x, y)) continue;
            if (grid.cells[x, y].type == CellType.Forest) continue;
            if (grid.cells[x, y].visibility == CellVisibility.Unseen) continue;
            if (grid.HasActiveLootAt(x, y)) continue;

            List<Vector2Int> path = Pathfinder.FindPath(grid, grid.player.gridX, grid.player.gridY, x, y);
            if (path == null || path.Count <= 1) continue;

            if (path.Count < bestPathLength)
            {
                bestPathLength = path.Count;
                moveTarget = new Vector2Int(x, y);
                foundTarget = true;
            }
        }

        return foundTarget;
    }

    void ClearPendingPickup()
    {
        hasPendingPickup = false;
        pendingPickupCell = Vector2Int.zero;
    }

    bool IsKnownHerbType(HerbType type)
    {
        switch (type)
        {
            case HerbType.BellLeaf:
            case HerbType.LavenderFern:
            case HerbType.ButtonRoot:
            case HerbType.HoneyClover:
            case HerbType.WarmNettle:
            case HerbType.SleepGrass:
            case HerbType.Glowberry:
                return true;
            default:
                return false;
        }
    }

    string GetHerbDisplayName(HerbType type)
    {
        switch (type)
        {
            case HerbType.BellLeaf:
                return "Bell Leaf";
            case HerbType.LavenderFern:
                return "Lavender Fern";
            case HerbType.ButtonRoot:
                return "Button Root";
            case HerbType.HoneyClover:
                return "Honey Clover";
            case HerbType.WarmNettle:
                return "Warm Nettle";
            case HerbType.SleepGrass:
                return "Sleep Grass";
            case HerbType.Glowberry:
                return "Glowberry";
            default:
                return "Herb";
        }
    }

    bool IsPointerOverUI(int pointerId = -1)
    {
        if (EventSystem.current == null)
            return false;

        if (pointerId >= 0)
            return EventSystem.current.IsPointerOverGameObject(pointerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    void OnValidate()
    {
        pickupRadius = Mathf.Max(0, pickupRadius);
    }
}
