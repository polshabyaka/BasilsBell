using UnityEngine;

public class RemedyInventory : MonoBehaviour
{
    [Header("Remedy Counts")]
    [SerializeField] int leafInfusionCount;
    [SerializeField] int lavenderTeaCount;
    [SerializeField] int sleepyInfusionCount;
    [SerializeField] int sweetDreamsTeaCount;
    [SerializeField] int strangeBrewCount;

    [Header("Debug")]
    [SerializeField] bool logAddsToConsole = true;

    public void AddRemedy(RemedyType type, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("AddRemedy needs a positive amount.", this);
            return;
        }

        if (type == RemedyType.None)
        {
            Debug.LogWarning("Cannot add RemedyType.None.", this);
            return;
        }

        int newCount = GetCount(type) + amount;
        SetCount(type, newCount);

        if (logAddsToConsole)
            Debug.Log(type + " +" + amount + " (total: " + newCount + ")", this);
    }

    public bool TrySpendRemedy(RemedyType type, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("TrySpendRemedy needs a positive amount.", this);
            return false;
        }

        if (type == RemedyType.None)
            return false;

        int currentCount = GetCount(type);
        if (currentCount < amount)
            return false;

        SetCount(type, currentCount - amount);
        return true;
    }

    public int GetCount(RemedyType type)
    {
        switch (type)
        {
            case RemedyType.LeafInfusion:
                return leafInfusionCount;
            case RemedyType.LavenderTea:
                return lavenderTeaCount;
            case RemedyType.SleepyInfusion:
                return sleepyInfusionCount;
            case RemedyType.SweetDreamsTea:
                return sweetDreamsTeaCount;
            case RemedyType.StrangeBrew:
                return strangeBrewCount;
            case RemedyType.None:
                return 0;
            default:
                Debug.LogWarning("Unknown remedy type: " + type, this);
                return 0;
        }
    }

    void SetCount(RemedyType type, int amount)
    {
        amount = Mathf.Max(0, amount);

        switch (type)
        {
            case RemedyType.LeafInfusion:
                leafInfusionCount = amount;
                break;
            case RemedyType.LavenderTea:
                lavenderTeaCount = amount;
                break;
            case RemedyType.SleepyInfusion:
                sleepyInfusionCount = amount;
                break;
            case RemedyType.SweetDreamsTea:
                sweetDreamsTeaCount = amount;
                break;
            case RemedyType.StrangeBrew:
                strangeBrewCount = amount;
                break;
            case RemedyType.None:
                break;
            default:
                Debug.LogWarning("Unknown remedy type: " + type, this);
                break;
        }
    }

    void OnValidate()
    {
        leafInfusionCount = Mathf.Max(0, leafInfusionCount);
        lavenderTeaCount = Mathf.Max(0, lavenderTeaCount);
        sleepyInfusionCount = Mathf.Max(0, sleepyInfusionCount);
        sweetDreamsTeaCount = Mathf.Max(0, sweetDreamsTeaCount);
        strangeBrewCount = Mathf.Max(0, strangeBrewCount);
    }
}
