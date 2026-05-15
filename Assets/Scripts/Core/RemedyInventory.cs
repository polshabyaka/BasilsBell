using UnityEngine;

public class RemedyInventory : MonoBehaviour
{
    [Header("Remedy Counts")]
    [SerializeField] int leafInfusionCount;
    [SerializeField] int lavenderTeaCount;
    [SerializeField] int rootTonicCount;
    [SerializeField] int honeySyrupCount;
    [SerializeField] int warmingTeaCount;
    [SerializeField] int sleepyInfusionCount;
    [SerializeField] int glowElixirCount;
    [SerializeField] int sweetDreamsTeaCount;
    [SerializeField] int honeyChildInfusionCount;
    [SerializeField] int warmChillTeaCount;
    [SerializeField] int thickWarmingDecoctionCount;
    [SerializeField] int strongColdDecoctionCount;
    [SerializeField] int brightBerryInfusionCount;
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
            case RemedyType.RootTonic:
                return rootTonicCount;
            case RemedyType.HoneySyrup:
                return honeySyrupCount;
            case RemedyType.WarmingTea:
                return warmingTeaCount;
            case RemedyType.SleepyInfusion:
                return sleepyInfusionCount;
            case RemedyType.GlowElixir:
                return glowElixirCount;
            case RemedyType.SweetDreamsTea:
                return sweetDreamsTeaCount;
            case RemedyType.HoneyChildInfusion:
                return honeyChildInfusionCount;
            case RemedyType.WarmChillTea:
                return warmChillTeaCount;
            case RemedyType.ThickWarmingDecoction:
                return thickWarmingDecoctionCount;
            case RemedyType.StrongColdDecoction:
                return strongColdDecoctionCount;
            case RemedyType.BrightBerryInfusion:
                return brightBerryInfusionCount;
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
            case RemedyType.RootTonic:
                rootTonicCount = amount;
                break;
            case RemedyType.HoneySyrup:
                honeySyrupCount = amount;
                break;
            case RemedyType.WarmingTea:
                warmingTeaCount = amount;
                break;
            case RemedyType.SleepyInfusion:
                sleepyInfusionCount = amount;
                break;
            case RemedyType.GlowElixir:
                glowElixirCount = amount;
                break;
            case RemedyType.SweetDreamsTea:
                sweetDreamsTeaCount = amount;
                break;
            case RemedyType.HoneyChildInfusion:
                honeyChildInfusionCount = amount;
                break;
            case RemedyType.WarmChillTea:
                warmChillTeaCount = amount;
                break;
            case RemedyType.ThickWarmingDecoction:
                thickWarmingDecoctionCount = amount;
                break;
            case RemedyType.StrongColdDecoction:
                strongColdDecoctionCount = amount;
                break;
            case RemedyType.BrightBerryInfusion:
                brightBerryInfusionCount = amount;
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
        rootTonicCount = Mathf.Max(0, rootTonicCount);
        honeySyrupCount = Mathf.Max(0, honeySyrupCount);
        warmingTeaCount = Mathf.Max(0, warmingTeaCount);
        sleepyInfusionCount = Mathf.Max(0, sleepyInfusionCount);
        glowElixirCount = Mathf.Max(0, glowElixirCount);
        sweetDreamsTeaCount = Mathf.Max(0, sweetDreamsTeaCount);
        honeyChildInfusionCount = Mathf.Max(0, honeyChildInfusionCount);
        warmChillTeaCount = Mathf.Max(0, warmChillTeaCount);
        thickWarmingDecoctionCount = Mathf.Max(0, thickWarmingDecoctionCount);
        strongColdDecoctionCount = Mathf.Max(0, strongColdDecoctionCount);
        brightBerryInfusionCount = Mathf.Max(0, brightBerryInfusionCount);
        strangeBrewCount = Mathf.Max(0, strangeBrewCount);
    }
}
