using UnityEngine;

public class HerbInventory : MonoBehaviour
{
    [Header("Herb Counts")]
    [SerializeField] int bellLeafCount;
    [SerializeField] int lavenderFernCount;
    [SerializeField] int buttonRootCount;
    [SerializeField] int honeyCloverCount;
    [SerializeField] int warmNettleCount;
    [SerializeField] int sleepGrassCount;
    [SerializeField] int glowberryCount;

    [Header("Debug")]
    [SerializeField] bool logAddsToConsole = true;

    public void AddHerb(HerbType type, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("AddHerb needs a positive amount.", this);
            return;
        }

        int newCount = GetCount(type) + amount;
        SetCount(type, newCount);

        if (logAddsToConsole)
            Debug.Log(type + " +" + amount + " (total: " + newCount + ")", this);
    }

    public bool TrySpendHerb(HerbType type, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("TrySpendHerb needs a positive amount.", this);
            return false;
        }

        int currentCount = GetCount(type);
        if (currentCount < amount)
            return false;

        SetCount(type, currentCount - amount);
        return true;
    }

    public int GetCount(HerbType type)
    {
        switch (type)
        {
            case HerbType.BellLeaf:
                return bellLeafCount;
            case HerbType.LavenderFern:
                return lavenderFernCount;
            case HerbType.ButtonRoot:
                return buttonRootCount;
            case HerbType.HoneyClover:
                return honeyCloverCount;
            case HerbType.WarmNettle:
                return warmNettleCount;
            case HerbType.SleepGrass:
                return sleepGrassCount;
            case HerbType.Glowberry:
                return glowberryCount;
            default:
                Debug.LogWarning("Unknown herb type: " + type, this);
                return 0;
        }
    }

    [ContextMenu("Print Herb Inventory")]
    public void PrintInventoryToConsole()
    {
        Debug.Log(GetDebugSummary(), this);
    }

    public string GetDebugSummary()
    {
        return "Herb Inventory - "
            + "BellLeaf: " + bellLeafCount
            + ", LavenderFern: " + lavenderFernCount
            + ", ButtonRoot: " + buttonRootCount
            + ", HoneyClover: " + honeyCloverCount
            + ", WarmNettle: " + warmNettleCount
            + ", SleepGrass: " + sleepGrassCount
            + ", Glowberry: " + glowberryCount;
    }

    void SetCount(HerbType type, int amount)
    {
        amount = Mathf.Max(0, amount);

        switch (type)
        {
            case HerbType.BellLeaf:
                bellLeafCount = amount;
                break;
            case HerbType.LavenderFern:
                lavenderFernCount = amount;
                break;
            case HerbType.ButtonRoot:
                buttonRootCount = amount;
                break;
            case HerbType.HoneyClover:
                honeyCloverCount = amount;
                break;
            case HerbType.WarmNettle:
                warmNettleCount = amount;
                break;
            case HerbType.SleepGrass:
                sleepGrassCount = amount;
                break;
            case HerbType.Glowberry:
                glowberryCount = amount;
                break;
            default:
                Debug.LogWarning("Unknown herb type: " + type, this);
                break;
        }
    }

    void OnValidate()
    {
        bellLeafCount = Mathf.Max(0, bellLeafCount);
        lavenderFernCount = Mathf.Max(0, lavenderFernCount);
        buttonRootCount = Mathf.Max(0, buttonRootCount);
        honeyCloverCount = Mathf.Max(0, honeyCloverCount);
        warmNettleCount = Mathf.Max(0, warmNettleCount);
        sleepGrassCount = Mathf.Max(0, sleepGrassCount);
        glowberryCount = Mathf.Max(0, glowberryCount);
    }
}
