using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCauldronController : MonoBehaviour
{
    const int MaxSelectedHerbs = 3;

    [Header("Inventories")]
    [SerializeField] HerbInventory herbInventory;
    [SerializeField] RemedyInventory remedyInventory;

    [Header("Reward Popup")]
    [SerializeField] SimpleRewardPopup rewardPopup;

    [Header("Herb Buttons")]
    [SerializeField] Button bellLeafButton;
    [SerializeField] Button lavenderFernButton;
    [SerializeField] Button buttonRootButton;
    [SerializeField] Button honeyCloverButton;
    [SerializeField] Button warmNettleButton;
    [SerializeField] Button sleepGrassButton;
    [SerializeField] Button glowberryButton;

    [Header("Herb Count Texts")]
    [SerializeField] TMP_Text bellLeafCountText;
    [SerializeField] TMP_Text lavenderFernCountText;
    [SerializeField] TMP_Text buttonRootCountText;
    [SerializeField] TMP_Text honeyCloverCountText;
    [SerializeField] TMP_Text warmNettleCountText;
    [SerializeField] TMP_Text sleepGrassCountText;
    [SerializeField] TMP_Text glowberryCountText;

    [Header("Slot Images")]
    [SerializeField] Image slot1Image;
    [SerializeField] Image slot2Image;
    [SerializeField] Image slot3Image;

    [Header("Optional Empty Slot Sprite")]
    [SerializeField] Sprite emptySlotSprite;

    [Header("Herb Sprites")]
    [SerializeField] Sprite bellLeafSprite;
    [SerializeField] Sprite lavenderFernSprite;
    [SerializeField] Sprite buttonRootSprite;
    [SerializeField] Sprite honeyCloverSprite;
    [SerializeField] Sprite warmNettleSprite;
    [SerializeField] Sprite sleepGrassSprite;
    [SerializeField] Sprite glowberrySprite;

    [Header("Control Buttons")]
    [SerializeField] Button brewButton;
    [SerializeField] Button clearSelectionButton;

    [Header("Optional Text")]
    [SerializeField] TMP_Text brewResultText;

    readonly List<HerbType> selectedHerbs = new List<HerbType>();

    Sprite slot1EmptySprite;
    Sprite slot2EmptySprite;
    Sprite slot3EmptySprite;
    bool cachedEmptySlotSprites;

    void Start()
    {
        CacheEmptySlotSprites();
        ConnectButtonListeners();
        RefreshUI();
    }

    void OnEnable()
    {
        CacheEmptySlotSprites();
        RefreshUI();
    }

    public void SelectHerb(HerbType type)
    {
        if (selectedHerbs.Count >= MaxSelectedHerbs)
            return;

        int ownedCount = GetOwnedCount(type);
        int selectedCount = GetSelectedCount(type);
        if (ownedCount <= selectedCount)
            return;

        selectedHerbs.Add(type);

        if (brewResultText != null)
            brewResultText.text = "";

        RefreshUI();
    }

    public void Brew()
    {
        if (selectedHerbs.Count == 0)
            return;

        if (herbInventory == null || remedyInventory == null)
        {
            if (brewResultText != null)
                brewResultText.text = "Missing inventory reference";

            RefreshUI();
            return;
        }

        if (!HasEnoughSelectedHerbs())
        {
            if (brewResultText != null)
                brewResultText.text = "Not enough herbs";

            RefreshUI();
            return;
        }

        RemedyType remedy = GetRemedyForSelection();
        int remedyAmount = GetRemedyAmountForSelection(remedy);
        for (int i = 0; i < selectedHerbs.Count; i++)
        {
            herbInventory.TrySpendHerb(selectedHerbs[i]);
        }

        remedyInventory.AddRemedy(remedy, remedyAmount);

        string remedyName = GetRemedyDisplayName(remedy);
        ClearSelection();

        if (brewResultText != null)
            brewResultText.text = FormatRewardName(remedyName, remedyAmount);

        if (rewardPopup != null)
            rewardPopup.Show(FormatRewardName(remedyName, remedyAmount));
    }

    public void ClearSelection()
    {
        selectedHerbs.Clear();

        if (brewResultText != null)
            brewResultText.text = "";

        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshCountTexts();
        RefreshSlots();
        RefreshButtons();
    }

    public void RefreshSlots()
    {
        SetSlotImage(slot1Image, GetSelectedSpriteAt(0), slot1EmptySprite);
        SetSlotImage(slot2Image, GetSelectedSpriteAt(1), slot2EmptySprite);
        SetSlotImage(slot3Image, GetSelectedSpriteAt(2), slot3EmptySprite);
    }

    public void RefreshCountTexts()
    {
        SetCountText(bellLeafCountText, HerbType.BellLeaf);
        SetCountText(lavenderFernCountText, HerbType.LavenderFern);
        SetCountText(buttonRootCountText, HerbType.ButtonRoot);
        SetCountText(honeyCloverCountText, HerbType.HoneyClover);
        SetCountText(warmNettleCountText, HerbType.WarmNettle);
        SetCountText(sleepGrassCountText, HerbType.SleepGrass);
        SetCountText(glowberryCountText, HerbType.Glowberry);
    }

    public RemedyType GetRemedyForSelection()
    {
        int bellLeaf = GetSelectedCount(HerbType.BellLeaf);
        int buttonRoot = GetSelectedCount(HerbType.ButtonRoot);
        int honeyClover = GetSelectedCount(HerbType.HoneyClover);
        int warmNettle = GetSelectedCount(HerbType.WarmNettle);
        int lavenderFern = GetSelectedCount(HerbType.LavenderFern);
        int sleepGrass = GetSelectedCount(HerbType.SleepGrass);
        int glowberry = GetSelectedCount(HerbType.Glowberry);

        if (IsSelectionAllSame())
            return GetSimpleRemedyForHerb(selectedHerbs[0]);

        if (selectedHerbs.Count == 3 && warmNettle == 1 && buttonRoot == 1 && honeyClover == 1)
            return RemedyType.StrongColdDecoction;

        if (selectedHerbs.Count == 3 && glowberry == 1 && honeyClover == 1 && bellLeaf == 1)
            return RemedyType.BrightBerryInfusion;

        if (selectedHerbs.Count == 2 && lavenderFern == 1 && sleepGrass == 1)
            return RemedyType.SweetDreamsTea;

        if (selectedHerbs.Count == 2 && bellLeaf == 1 && honeyClover == 1)
            return RemedyType.HoneyChildInfusion;

        if (selectedHerbs.Count == 2 && warmNettle == 1 && honeyClover == 1)
            return RemedyType.WarmChillTea;

        if (selectedHerbs.Count == 2 && warmNettle == 1 && buttonRoot == 1)
            return RemedyType.ThickWarmingDecoction;

        return RemedyType.StrangeBrew;
    }

    public Sprite GetHerbSprite(HerbType type)
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

    public string GetHerbDisplayName(HerbType type)
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
                return "Unknown Herb";
        }
    }

    public string GetRemedyDisplayName(RemedyType type)
    {
        switch (type)
        {
            case RemedyType.LeafInfusion:
                return "Leaf Infusion";
            case RemedyType.LavenderTea:
                return "Lavender Tea";
            case RemedyType.RootTonic:
                return "Root Tonic";
            case RemedyType.HoneySyrup:
                return "Honey Syrup";
            case RemedyType.WarmingTea:
                return "Warming Tea";
            case RemedyType.SleepyInfusion:
                return "Sleepy Infusion";
            case RemedyType.GlowElixir:
                return "Glow Elixir";
            case RemedyType.SweetDreamsTea:
                return "Sweet Dreams Tea";
            case RemedyType.HoneyChildInfusion:
                return "Honey Child Infusion";
            case RemedyType.WarmChillTea:
                return "Warm Chill Tea";
            case RemedyType.ThickWarmingDecoction:
                return "Thick Warming Decoction";
            case RemedyType.StrongColdDecoction:
                return "Strong Cold Decoction";
            case RemedyType.BrightBerryInfusion:
                return "Bright Berry Infusion";
            case RemedyType.StrangeBrew:
                return "Strange Brew";
            default:
                return "Unknown Remedy";
        }
    }

    void ConnectButtonListeners()
    {
        if (bellLeafButton != null)
            bellLeafButton.onClick.AddListener(() => SelectHerb(HerbType.BellLeaf));

        if (lavenderFernButton != null)
            lavenderFernButton.onClick.AddListener(() => SelectHerb(HerbType.LavenderFern));

        if (buttonRootButton != null)
            buttonRootButton.onClick.AddListener(() => SelectHerb(HerbType.ButtonRoot));

        if (honeyCloverButton != null)
            honeyCloverButton.onClick.AddListener(() => SelectHerb(HerbType.HoneyClover));

        if (warmNettleButton != null)
            warmNettleButton.onClick.AddListener(() => SelectHerb(HerbType.WarmNettle));

        if (sleepGrassButton != null)
            sleepGrassButton.onClick.AddListener(() => SelectHerb(HerbType.SleepGrass));

        if (glowberryButton != null)
            glowberryButton.onClick.AddListener(() => SelectHerb(HerbType.Glowberry));

        if (brewButton != null)
            brewButton.onClick.AddListener(Brew);

        if (clearSelectionButton != null)
            clearSelectionButton.onClick.AddListener(ClearSelection);
    }

    void RefreshButtons()
    {
        SetHerbButtonInteractable(bellLeafButton, HerbType.BellLeaf);
        SetHerbButtonInteractable(lavenderFernButton, HerbType.LavenderFern);
        SetHerbButtonInteractable(buttonRootButton, HerbType.ButtonRoot);
        SetHerbButtonInteractable(honeyCloverButton, HerbType.HoneyClover);
        SetHerbButtonInteractable(warmNettleButton, HerbType.WarmNettle);
        SetHerbButtonInteractable(sleepGrassButton, HerbType.SleepGrass);
        SetHerbButtonInteractable(glowberryButton, HerbType.Glowberry);

        if (brewButton != null)
            brewButton.interactable = selectedHerbs.Count > 0;

        if (clearSelectionButton != null)
            clearSelectionButton.interactable = selectedHerbs.Count > 0;
    }

    void SetHerbButtonInteractable(Button button, HerbType type)
    {
        if (button == null) return;

        int ownedCount = GetOwnedCount(type);
        int selectedCount = GetSelectedCount(type);
        button.interactable = selectedHerbs.Count < MaxSelectedHerbs && ownedCount > selectedCount;
    }

    void SetCountText(TMP_Text countText, HerbType type)
    {
        if (countText != null)
            countText.text = "x" + GetOwnedCount(type);
    }

    void SetSlotImage(Image slotImage, Sprite herbSprite, Sprite fallbackEmptySprite)
    {
        if (slotImage == null) return;

        if (herbSprite != null)
        {
            slotImage.sprite = herbSprite;
            slotImage.enabled = true;
            return;
        }

        Sprite emptySprite = emptySlotSprite != null ? emptySlotSprite : fallbackEmptySprite;
        slotImage.sprite = emptySprite;
        slotImage.enabled = true;
    }

    Sprite GetSelectedSpriteAt(int index)
    {
        if (index < 0 || index >= selectedHerbs.Count)
            return null;

        return GetHerbSprite(selectedHerbs[index]);
    }

    int GetRemedyAmountForSelection(RemedyType remedy)
    {
        if (remedy == RemedyType.StrangeBrew)
            return 1;

        if (IsSelectionAllSame())
            return selectedHerbs.Count;

        return 1;
    }

    RemedyType GetSimpleRemedyForHerb(HerbType type)
    {
        switch (type)
        {
            case HerbType.BellLeaf:
                return RemedyType.LeafInfusion;
            case HerbType.LavenderFern:
                return RemedyType.LavenderTea;
            case HerbType.ButtonRoot:
                return RemedyType.RootTonic;
            case HerbType.HoneyClover:
                return RemedyType.HoneySyrup;
            case HerbType.WarmNettle:
                return RemedyType.WarmingTea;
            case HerbType.SleepGrass:
                return RemedyType.SleepyInfusion;
            case HerbType.Glowberry:
                return RemedyType.GlowElixir;
            default:
                return RemedyType.StrangeBrew;
        }
    }

    bool IsSelectionAllSame()
    {
        if (selectedHerbs.Count == 0)
            return false;

        HerbType firstType = selectedHerbs[0];
        for (int i = 1; i < selectedHerbs.Count; i++)
        {
            if (selectedHerbs[i] != firstType)
                return false;
        }

        return true;
    }

    string FormatRewardName(string displayName, int amount)
    {
        if (amount <= 1)
            return displayName;

        return displayName + " x" + amount;
    }

    int GetOwnedCount(HerbType type)
    {
        if (herbInventory == null)
            return 0;

        return herbInventory.GetCount(type);
    }

    int GetSelectedCount(HerbType type)
    {
        int count = 0;

        for (int i = 0; i < selectedHerbs.Count; i++)
        {
            if (selectedHerbs[i] == type)
                count++;
        }

        return count;
    }

    bool HasEnoughSelectedHerbs()
    {
        if (herbInventory == null)
            return false;

        return GetSelectedCount(HerbType.BellLeaf) <= herbInventory.GetCount(HerbType.BellLeaf)
            && GetSelectedCount(HerbType.LavenderFern) <= herbInventory.GetCount(HerbType.LavenderFern)
            && GetSelectedCount(HerbType.ButtonRoot) <= herbInventory.GetCount(HerbType.ButtonRoot)
            && GetSelectedCount(HerbType.HoneyClover) <= herbInventory.GetCount(HerbType.HoneyClover)
            && GetSelectedCount(HerbType.WarmNettle) <= herbInventory.GetCount(HerbType.WarmNettle)
            && GetSelectedCount(HerbType.SleepGrass) <= herbInventory.GetCount(HerbType.SleepGrass)
            && GetSelectedCount(HerbType.Glowberry) <= herbInventory.GetCount(HerbType.Glowberry);
    }

    void CacheEmptySlotSprites()
    {
        if (cachedEmptySlotSprites)
            return;

        if (slot1Image != null)
            slot1EmptySprite = slot1Image.sprite;

        if (slot2Image != null)
            slot2EmptySprite = slot2Image.sprite;

        if (slot3Image != null)
            slot3EmptySprite = slot3Image.sprite;

        cachedEmptySlotSprites = true;
    }
}
