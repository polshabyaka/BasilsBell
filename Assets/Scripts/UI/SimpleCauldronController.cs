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

    [Header("Brew Timing")]
    [SerializeField] Vector2 brewTimeSeconds = new Vector2(10f, 15f);

    [Header("Cauldron Visuals")]
    [SerializeField] Image cauldronImage;
    [SerializeField] Sprite idleCauldronSprite;
    [SerializeField] Sprite brewingCauldronSprite;
    [SerializeField] Sprite readyCauldronSprite;
    [SerializeField] float readyVisualSeconds = 2f;

    [Header("Smoke Visuals")]
    [SerializeField] bool createSmokeImageIfMissing = true;
    [SerializeField] Image smokeImage;
    [SerializeField] Sprite smokeSpriteA;
    [SerializeField] Sprite smokeSpriteB;
    [SerializeField] float smokeFrameSeconds = 0.35f;
    [SerializeField] Vector2 generatedSmokeOffset = new Vector2(0f, 120f);
    [SerializeField] Vector2 generatedSmokeSize = new Vector2(180f, 180f);

    [Header("Shop Panel Visuals")]
    [SerializeField] Image shopCauldronImage;
    [SerializeField] bool createShopSmokeImageIfMissing = true;
    [SerializeField] Image shopSmokeImage;
    [SerializeField] Vector2 generatedShopSmokeOffset = new Vector2(0f, 120f);
    [SerializeField] Vector2 generatedShopSmokeSize = new Vector2(180f, 180f);

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
    Sprite defaultCauldronSprite;
    Sprite defaultSmokeSprite;
    Sprite defaultShopCauldronSprite;
    Sprite defaultShopSmokeSprite;
    bool cachedEmptySlotSprites;
    bool cachedVisualSprites;

    bool isBrewing;
    float brewCompleteTime;
    float readyVisualEndTime;
    RemedyType pendingRemedy;
    int pendingRemedyAmount;
    string pendingRewardName;
    BrewTicker brewTicker;

    void Start()
    {
        CacheSprites();
        ConnectButtonListeners();
        RefreshUI();
    }

    void OnEnable()
    {
        CacheSprites();

        if (isBrewing)
            EnsureBrewTicker();

        if (isBrewing && Time.time >= brewCompleteTime)
        {
            FinishBrew();
            return;
        }

        RefreshUI();
    }

    void Update()
    {
        TickBrew();
    }

    void OnDestroy()
    {
        StopBrewTicker();
    }

    void TickBrew()
    {
        if (isBrewing)
        {
            float remainingSeconds = brewCompleteTime - Time.time;
            if (remainingSeconds <= 0f)
            {
                FinishBrew();
                return;
            }

            RefreshBrewTimerText(remainingSeconds);
            RefreshVisuals();
            return;
        }

        if (readyVisualEndTime > 0f && Time.time >= readyVisualEndTime)
        {
            readyVisualEndTime = 0f;
            RefreshVisuals();
        }
    }

    public void SelectHerb(HerbType type)
    {
        if (isBrewing)
            return;

        if (selectedHerbs.Count >= MaxSelectedHerbs)
            return;

        int ownedCount = GetOwnedCount(type);
        int selectedCount = GetSelectedCount(type);
        if (ownedCount <= selectedCount)
            return;

        selectedHerbs.Add(type);

        if (brewResultText != null)
            SetBrewResultText("", false);

        RefreshUI();
    }

    public void Brew()
    {
        if (isBrewing)
            return;

        if (selectedHerbs.Count == 0)
            return;

        if (herbInventory == null || remedyInventory == null)
        {
            SetBrewResultText("Missing inventory reference", true);

            RefreshUI();
            return;
        }

        if (!HasEnoughSelectedHerbs())
        {
            SetBrewResultText("Not enough herbs", true);

            RefreshUI();
            return;
        }

        pendingRemedy = GetRemedyForSelection();
        pendingRemedyAmount = GetRemedyAmountForSelection(pendingRemedy);
        pendingRewardName = FormatRewardName(GetRemedyDisplayName(pendingRemedy), pendingRemedyAmount);

        for (int i = 0; i < selectedHerbs.Count; i++)
        {
            herbInventory.TrySpendHerb(selectedHerbs[i]);
        }

        isBrewing = true;
        brewCompleteTime = Time.time + GetRandomBrewDuration();
        readyVisualEndTime = 0f;

        EnsureBrewTicker();
        RefreshUI();
        RefreshBrewTimerText(brewCompleteTime - Time.time);
    }

    void FinishBrew()
    {
        if (!isBrewing)
            return;

        isBrewing = false;
        readyVisualEndTime = Time.time + Mathf.Max(0f, readyVisualSeconds);

        if (remedyInventory != null)
            remedyInventory.AddRemedy(pendingRemedy, pendingRemedyAmount);

        selectedHerbs.Clear();

        //SetBrewResultText(pendingRewardName + " is ready", true);

        if (rewardPopup != null)
            rewardPopup.Show("Potion Ready", pendingRewardName);

        RefreshUI();
    }

    public void ClearSelection()
    {
        if (isBrewing)
            return;

        selectedHerbs.Clear();

        SetBrewResultText("", false);

        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshCountTexts();
        RefreshSlots();
        RefreshButtons();
        RefreshVisuals();
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
            brewButton.interactable = !isBrewing && selectedHerbs.Count > 0;

        if (clearSelectionButton != null)
            clearSelectionButton.interactable = !isBrewing && selectedHerbs.Count > 0;
    }

    void SetHerbButtonInteractable(Button button, HerbType type)
    {
        if (button == null) return;

        if (isBrewing)
        {
            button.interactable = false;
            return;
        }

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

    void RefreshBrewTimerText(float remainingSeconds)
    {
        int shownSeconds = Mathf.Max(1, Mathf.CeilToInt(remainingSeconds));
        SetBrewResultText("Brewing... " + shownSeconds + "s", true);
    }

    void SetBrewResultText(string text, bool visible)
    {
        if (brewResultText == null)
            return;

        brewResultText.text = text;
        brewResultText.gameObject.SetActive(visible);
    }

    void RefreshVisuals()
    {
        RefreshCauldronVisual(cauldronImage, defaultCauldronSprite);
        RefreshCauldronVisual(shopCauldronImage, defaultShopCauldronSprite);
        RefreshSmokeVisual(smokeImage);
        RefreshSmokeVisual(shopSmokeImage);
    }

    void RefreshCauldronVisual(Image targetImage, Sprite fallbackSprite)
    {
        if (targetImage == null)
            return;

        if (isBrewing)
        {
            SetImageSprite(targetImage, brewingCauldronSprite, idleCauldronSprite, fallbackSprite);
            return;
        }

        if (readyVisualEndTime > 0f && Time.time < readyVisualEndTime)
        {
            SetImageSprite(targetImage, readyCauldronSprite, brewingCauldronSprite, idleCauldronSprite, fallbackSprite);
            return;
        }

        SetImageSprite(targetImage, idleCauldronSprite, fallbackSprite);
    }

    void RefreshSmokeVisual(Image targetImage)
    {
        if (targetImage == null)
            return;

        if (!isBrewing)
        {
            targetImage.enabled = false;
            return;
        }

        Sprite smokeSprite = GetCurrentSmokeSprite();
        targetImage.sprite = smokeSprite;
        targetImage.enabled = smokeSprite != null;
    }

    Sprite GetCurrentSmokeSprite()
    {
        float frameSeconds = Mathf.Max(0.05f, smokeFrameSeconds);
        bool useSecondSprite = Mathf.FloorToInt(Time.time / frameSeconds) % 2 == 1;

        if (useSecondSprite && smokeSpriteB != null)
            return smokeSpriteB;

        if (smokeSpriteA != null)
            return smokeSpriteA;

        if (smokeSpriteB != null)
            return smokeSpriteB;

        return defaultSmokeSprite;
    }

    void SetImageSprite(Image image, params Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null)
                continue;

            image.sprite = sprites[i];
            image.enabled = true;
            return;
        }
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

    float GetRandomBrewDuration()
    {
        float minSeconds = Mathf.Max(0.1f, Mathf.Min(brewTimeSeconds.x, brewTimeSeconds.y));
        float maxSeconds = Mathf.Max(minSeconds, Mathf.Max(brewTimeSeconds.x, brewTimeSeconds.y));
        return Random.Range(minSeconds, maxSeconds);
    }

    void CacheSprites()
    {
        CacheEmptySlotSprites();
        CacheVisualSprites();
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

    void CacheVisualSprites()
    {
        if (cachedVisualSprites)
            return;

        if (cauldronImage != null)
            defaultCauldronSprite = cauldronImage.sprite;

        if (shopCauldronImage != null)
            defaultShopCauldronSprite = shopCauldronImage.sprite;

        EnsureSmokeImages();

        if (smokeImage != null)
            defaultSmokeSprite = smokeImage.sprite;

        if (shopSmokeImage != null)
            defaultShopSmokeSprite = shopSmokeImage.sprite;

        cachedVisualSprites = true;
    }

    void EnsureSmokeImages()
    {
        EnsureSmokeImage(ref smokeImage, cauldronImage, createSmokeImageIfMissing, generatedSmokeOffset, generatedSmokeSize, "GeneratedSmokeImage");
        EnsureSmokeImage(ref shopSmokeImage, shopCauldronImage, createShopSmokeImageIfMissing, generatedShopSmokeOffset, generatedShopSmokeSize, "GeneratedShopSmokeImage");
    }

    void EnsureSmokeImage(ref Image targetImage, Image parentImage, bool createIfMissing, Vector2 offset, Vector2 size, string objectName)
    {
        if (targetImage != null)
            return;

        if (!createIfMissing || parentImage == null)
            return;

        GameObject smokeObject = new GameObject(objectName, typeof(RectTransform));
        smokeObject.layer = parentImage.gameObject.layer;
        smokeObject.transform.SetParent(parentImage.transform, false);

        RectTransform smokeRect = smokeObject.GetComponent<RectTransform>();
        smokeRect.anchorMin = new Vector2(0.5f, 0.5f);
        smokeRect.anchorMax = new Vector2(0.5f, 0.5f);
        smokeRect.pivot = new Vector2(0.5f, 0.5f);
        smokeRect.anchoredPosition = offset;
        smokeRect.sizeDelta = size;

        targetImage = smokeObject.AddComponent<Image>();
        targetImage.raycastTarget = false;
        targetImage.preserveAspect = true;
        targetImage.enabled = false;
    }

    void EnsureBrewTicker()
    {
        if (brewTicker != null)
            return;

        Transform parent = null;
        Canvas canvas = GetComponentInParent<Canvas>(true);
        if (canvas != null)
            parent = canvas.transform;

        GameObject tickerObject = new GameObject("CauldronBrewTicker");
        tickerObject.transform.SetParent(parent, false);
        brewTicker = tickerObject.AddComponent<BrewTicker>();
        brewTicker.Initialize(this);
    }

    bool ShouldKeepBrewTickerAlive()
    {
        return isBrewing || readyVisualEndTime > 0f;
    }

    void StopBrewTicker()
    {
        if (brewTicker == null)
            return;

        BrewTicker ticker = brewTicker;
        brewTicker = null;

        if (ticker != null)
            Destroy(ticker.gameObject);
    }

    void OnValidate()
    {
        brewTimeSeconds.x = Mathf.Max(0.1f, brewTimeSeconds.x);
        brewTimeSeconds.y = Mathf.Max(0.1f, brewTimeSeconds.y);
        smokeFrameSeconds = Mathf.Max(0.05f, smokeFrameSeconds);
        readyVisualSeconds = Mathf.Max(0f, readyVisualSeconds);
        generatedSmokeSize.x = Mathf.Max(1f, generatedSmokeSize.x);
        generatedSmokeSize.y = Mathf.Max(1f, generatedSmokeSize.y);
        generatedShopSmokeSize.x = Mathf.Max(1f, generatedShopSmokeSize.x);
        generatedShopSmokeSize.y = Mathf.Max(1f, generatedShopSmokeSize.y);
    }

    class BrewTicker : MonoBehaviour
    {
        SimpleCauldronController controller;

        public void Initialize(SimpleCauldronController controller)
        {
            this.controller = controller;
        }

        void Update()
        {
            if (controller == null)
            {
                DestroyTicker();
                return;
            }

            if (!controller.ShouldKeepBrewTickerAlive())
            {
                DestroyTicker();
                return;
            }

            if (!controller.isActiveAndEnabled)
                controller.TickBrew();
        }

        void DestroyTicker()
        {
            if (controller != null && controller.brewTicker == this)
                controller.brewTicker = null;

            Destroy(gameObject);
        }
    }
}
