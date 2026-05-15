using UnityEngine;

public class SimpleShelfController : MonoBehaviour
{
    [Header("Inventories")]
    [SerializeField] HerbInventory herbInventory;
    [SerializeField] RemedyInventory remedyInventory;

    [Header("Generated Slots")]
    [SerializeField] Transform slotRoot;
    [SerializeField] SimpleShelfSlotView slotPrefab;
    [SerializeField] GameObject emptyMessage;

    [Header("Remedy Sprites")]
    [SerializeField] Sprite leafInfusionSprite;
    [SerializeField] Sprite lavenderTeaSprite;
    [SerializeField] Sprite sleepyInfusionSprite;
    [SerializeField] Sprite sweetDreamsTeaSprite;
    [SerializeField] Sprite strangeBrewSprite;

    [Header("Herb Sprites")]
    [SerializeField] Sprite bellLeafSprite;
    [SerializeField] Sprite lavenderFernSprite;
    [SerializeField] Sprite buttonRootSprite;
    [SerializeField] Sprite honeyCloverSprite;
    [SerializeField] Sprite warmNettleSprite;
    [SerializeField] Sprite sleepGrassSprite;
    [SerializeField] Sprite glowberrySprite;

    bool started;

    void Start()
    {
        started = true;
        RefreshUI();
    }

    void OnEnable()
    {
        if (started)
            RefreshUI();
    }

    public void RefreshUI()
    {
        ClearSlots();

        int shownCount = 0;

        shownCount += AddRemedySlotIfOwned(RemedyType.LeafInfusion);
        shownCount += AddRemedySlotIfOwned(RemedyType.LavenderTea);
        shownCount += AddRemedySlotIfOwned(RemedyType.SleepyInfusion);
        shownCount += AddRemedySlotIfOwned(RemedyType.SweetDreamsTea);
        shownCount += AddRemedySlotIfOwned(RemedyType.StrangeBrew);

        shownCount += AddHerbSlotIfOwned(HerbType.BellLeaf);
        shownCount += AddHerbSlotIfOwned(HerbType.LavenderFern);
        shownCount += AddHerbSlotIfOwned(HerbType.ButtonRoot);
        shownCount += AddHerbSlotIfOwned(HerbType.HoneyClover);
        shownCount += AddHerbSlotIfOwned(HerbType.WarmNettle);
        shownCount += AddHerbSlotIfOwned(HerbType.SleepGrass);
        shownCount += AddHerbSlotIfOwned(HerbType.Glowberry);

        if (emptyMessage != null)
            emptyMessage.SetActive(shownCount == 0);
    }

    int AddRemedySlotIfOwned(RemedyType type)
    {
        if (remedyInventory == null)
            return 0;

        int count = remedyInventory.GetCount(type);
        if (count <= 0)
            return 0;

        AddSlot(GetRemedySprite(type), GetRemedyDisplayName(type), count);
        return 1;
    }

    int AddHerbSlotIfOwned(HerbType type)
    {
        if (herbInventory == null)
            return 0;

        int count = herbInventory.GetCount(type);
        if (count <= 0)
            return 0;

        AddSlot(GetHerbSprite(type), GetHerbDisplayName(type), count);
        return 1;
    }

    void AddSlot(Sprite icon, string displayName, int count)
    {
        if (slotRoot == null || slotPrefab == null)
            return;

        SimpleShelfSlotView slot = Instantiate(slotPrefab, slotRoot);
        slot.gameObject.SetActive(true);
        slot.SetItem(icon, displayName, count);
    }

    void ClearSlots()
    {
        if (slotRoot == null)
            return;

        for (int i = slotRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(slotRoot.GetChild(i).gameObject);
        }
    }

    Sprite GetRemedySprite(RemedyType type)
    {
        switch (type)
        {
            case RemedyType.LeafInfusion:
                return leafInfusionSprite;
            case RemedyType.LavenderTea:
                return lavenderTeaSprite;
            case RemedyType.SleepyInfusion:
                return sleepyInfusionSprite;
            case RemedyType.SweetDreamsTea:
                return sweetDreamsTeaSprite;
            case RemedyType.StrangeBrew:
                return strangeBrewSprite;
            default:
                return null;
        }
    }

    Sprite GetHerbSprite(HerbType type)
    {
        switch (type)
        {
            case HerbType.BellLeaf:
                return bellLeafSprite;
            case HerbType.LavenderFern:
                return lavenderFernSprite;
            case HerbType.ButtonRoot:
                return buttonRootSprite;
            case HerbType.HoneyClover:
                return honeyCloverSprite;
            case HerbType.WarmNettle:
                return warmNettleSprite;
            case HerbType.SleepGrass:
                return sleepGrassSprite;
            case HerbType.Glowberry:
                return glowberrySprite;
            default:
                return null;
        }
    }

    string GetRemedyDisplayName(RemedyType type)
    {
        switch (type)
        {
            case RemedyType.LeafInfusion:
                return "Leaf Infusion";
            case RemedyType.LavenderTea:
                return "Lavender Tea";
            case RemedyType.SleepyInfusion:
                return "Sleepy Infusion";
            case RemedyType.SweetDreamsTea:
                return "Sweet Dreams Tea";
            case RemedyType.StrangeBrew:
                return "Strange Brew";
            default:
                return "Remedy";
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
}
