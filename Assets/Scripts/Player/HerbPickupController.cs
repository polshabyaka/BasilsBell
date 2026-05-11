using UnityEngine;
using UnityEngine.EventSystems;

public class HerbPickupController : MonoBehaviour
{
    [SerializeField] GridManager grid;
    [SerializeField] HerbInventory inventory;
    [SerializeField] Camera worldCamera;
    [SerializeField] int pickupRadius = 1;
    [SerializeField] HerbType fallbackHerbType = HerbType.BellLeaf;

    void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    void Update()
    {
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
        int radius = Mathf.Max(0, pickupRadius);

        if (TryPickupAt(playerX, playerY))
            return true;

        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                if (TryPickupAt(playerX + dx, playerY + dy))
                    return true;
            }
        }

        return false;
    }

    void HandleTapOrClickPickup()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended && !IsPointerOverUI(touch.fingerId))
                TryPickupFromScreenPosition(touch.position);

            return;
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            TryPickupFromScreenPosition(Input.mousePosition);
    }

    bool TryPickupFromScreenPosition(Vector2 screenPosition)
    {
        if (!CanTryPickup()) return false;
        if (worldCamera == null) return false;

        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        if (!grid.TryWorldToGrid(worldPosition, out int x, out int y))
            return false;

        if (!IsWithinPickupRadius(x, y))
            return false;

        return TryPickupAt(x, y);
    }

    bool TryPickupAt(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return false;

        if (!grid.TryPickupHerbAt(x, y, out HerbType pickedType))
            return false;

        if (!IsKnownHerbType(pickedType))
            pickedType = fallbackHerbType;

        inventory.AddHerb(pickedType);
        return true;
    }

    bool IsWithinPickupRadius(int x, int y)
    {
        int radius = Mathf.Max(0, pickupRadius);
        int dx = Mathf.Abs(x - grid.player.gridX);
        int dy = Mathf.Abs(y - grid.player.gridY);
        return dx <= radius && dy <= radius;
    }

    bool IsInsideGrid(int x, int y)
    {
        if (grid == null) return false;
        return x >= 0 && x < grid.width && y >= 0 && y < grid.height;
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
