using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleOrderController : MonoBehaviour
{
    [System.Serializable]
    class DemoOrder
    {
        public string customerName = "Ivy";
        public RemedyType remedyType = RemedyType.LeafInfusion;
        public int amount = 1;
        public Sprite remedySprite;
        [TextArea] public string customDetailText;
        public bool completed;
    }

    [Header("Inventory")]
    [SerializeField] RemedyInventory remedyInventory;

    [Header("Order Cards")]
    [SerializeField] Transform orderRoot;
    [SerializeField] SimpleOrderCardView orderCardPrefab;
    [SerializeField] GameObject emptyMessage;
    [SerializeField] Sprite paperSpriteA;
    [SerializeField] Sprite paperSpriteB;

    [Header("Orders")]
    [SerializeField] DemoOrder[] orders =
    {
        new DemoOrder
        {
            customerName = "Ivy",
            remedyType = RemedyType.LeafInfusion,
            amount = 2
        }
    };

    [Header("Detail Window")]
    [SerializeField] GameObject detailWindow;
    [SerializeField] TMP_Text detailTitleText;
    [SerializeField] TMP_Text detailBodyText;
    [SerializeField] TMP_Text detailStatusText;
    [SerializeField] Image detailRemedyImage;
    [SerializeField] Button fulfillButton;
    [SerializeField] Button closeDetailButton;

    [Header("Optional Reward Popup")]
    [SerializeField] SimpleRewardPopup rewardPopup;

    int selectedOrderIndex = -1;
    bool started;

    void Awake()
    {
        ResolveRemedyInventory();
    }

    void Start()
    {
        started = true;
        ConnectButtons();
        CloseDetail();
        RefreshUI();
    }

    void OnEnable()
    {
        if (!started)
            return;

        CloseDetail();
        RefreshUI();
    }

    public void RefreshUI()
    {
        ResolveRemedyInventory();
        ClearCards();

        int activeOrderCount = 0;

        if (orders != null)
        {
            for (int i = 0; i < orders.Length; i++)
            {
                DemoOrder order = orders[i];
                if (order == null || order.completed)
                    continue;

                int index = i;
                AddOrderCard(order, index, activeOrderCount);
                activeOrderCount++;
            }
        }

        if (emptyMessage != null)
            emptyMessage.SetActive(activeOrderCount == 0);
    }

    public void CloseDetail()
    {
        selectedOrderIndex = -1;

        if (detailWindow != null)
            detailWindow.SetActive(false);
    }

    void ConnectButtons()
    {
        if (fulfillButton != null)
            fulfillButton.onClick.AddListener(FulfillSelectedOrder);

        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(CloseDetail);
    }

    void AddOrderCard(DemoOrder order, int orderIndex, int visibleIndex)
    {
        if (orderRoot == null || orderCardPrefab == null)
            return;

        SimpleOrderCardView card = Instantiate(orderCardPrefab, orderRoot);
        card.gameObject.SetActive(true);

        Sprite paperSprite = GetPaperSprite(visibleIndex);
        card.SetOrder(paperSprite, order.remedySprite, order.amount, order.completed, () => OpenDetail(orderIndex));
    }

    void OpenDetail(int orderIndex)
    {
        if (!IsValidOrderIndex(orderIndex))
            return;

        selectedOrderIndex = orderIndex;
        DemoOrder order = orders[orderIndex];

        if (detailWindow != null)
            detailWindow.SetActive(true);

        if (detailTitleText != null)
            detailTitleText.text = order.customerName + "'s Order";

        if (detailBodyText != null)
            detailBodyText.text = GetDetailText(order);

        if (detailRemedyImage != null)
        {
            detailRemedyImage.sprite = order.remedySprite;
            detailRemedyImage.enabled = order.remedySprite != null;
        }

        RefreshDetailStatus();
    }

    void FulfillSelectedOrder()
    {
        if (!IsValidOrderIndex(selectedOrderIndex))
            return;

        ResolveRemedyInventory();

        DemoOrder order = orders[selectedOrderIndex];
        if (order.completed)
            return;

        if (remedyInventory == null || !remedyInventory.TrySpendRemedy(order.remedyType, order.amount))
        {
            RefreshDetailStatus();
            return;
        }

        order.completed = true;

        if (rewardPopup != null)
            rewardPopup.Show("Order Complete");

        CloseDetail();
        RefreshUI();
    }

    void RefreshDetailStatus()
    {
        if (!IsValidOrderIndex(selectedOrderIndex))
            return;

        ResolveRemedyInventory();

        DemoOrder order = orders[selectedOrderIndex];
        int ownedCount = remedyInventory != null ? remedyInventory.GetCount(order.remedyType) : 0;
        bool canFulfill = ownedCount >= order.amount && !order.completed;

        if (fulfillButton != null)
            fulfillButton.interactable = canFulfill;

        if (detailStatusText != null)
        {
            if (order.completed)
                detailStatusText.text = "Complete";
            else
                detailStatusText.text = "You have x" + ownedCount + " / x" + order.amount;
        }
    }

    void ClearCards()
    {
        if (orderRoot == null)
            return;

        for (int i = orderRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(orderRoot.GetChild(i).gameObject);
        }
    }

    bool IsValidOrderIndex(int orderIndex)
    {
        return orders != null
            && orderIndex >= 0
            && orderIndex < orders.Length
            && orders[orderIndex] != null;
    }

    Sprite GetPaperSprite(int visibleIndex)
    {
        if (paperSpriteB != null && visibleIndex % 2 == 1)
            return paperSpriteB;

        return paperSpriteA;
    }

    string GetDetailText(DemoOrder order)
    {
        if (!string.IsNullOrWhiteSpace(order.customDetailText))
            return order.customDetailText;

        return order.customerName + " needs " + order.amount + " " + GetRemedyDisplayName(order.remedyType, order.amount) + ".";
    }

    string GetRemedyDisplayName(RemedyType type, int amount)
    {
        string displayName;

        switch (type)
        {
            case RemedyType.LeafInfusion:
                displayName = "Leaf Infusion";
                break;
            case RemedyType.LavenderTea:
                displayName = "Lavender Tea";
                break;
            case RemedyType.RootTonic:
                displayName = "Root Tonic";
                break;
            case RemedyType.HoneySyrup:
                displayName = "Honey Syrup";
                break;
            case RemedyType.WarmingTea:
                displayName = "Warming Tea";
                break;
            case RemedyType.SleepyInfusion:
                displayName = "Sleepy Infusion";
                break;
            case RemedyType.GlowElixir:
                displayName = "Glow Elixir";
                break;
            case RemedyType.SweetDreamsTea:
                displayName = "Sweet Dreams Tea";
                break;
            case RemedyType.HoneyChildInfusion:
                displayName = "Honey Child Infusion";
                break;
            case RemedyType.WarmChillTea:
                displayName = "Warm Chill Tea";
                break;
            case RemedyType.ThickWarmingDecoction:
                displayName = "Thick Warming Decoction";
                break;
            case RemedyType.StrongColdDecoction:
                displayName = "Strong Cold Decoction";
                break;
            case RemedyType.BrightBerryInfusion:
                displayName = "Bright Berry Infusion";
                break;
            case RemedyType.StrangeBrew:
                displayName = "Strange Brew";
                break;
            default:
                displayName = "Remedy";
                break;
        }

        if (amount == 1)
            return displayName;

        return displayName + "s";
    }

    void ResolveRemedyInventory()
    {
        if (remedyInventory != null)
            return;

        remedyInventory = FindFirstObjectByType<RemedyInventory>();
    }

    void OnValidate()
    {
        EnsureDefaultOrderIfEmpty();

        if (orders == null)
            return;

        for (int i = 0; i < orders.Length; i++)
        {
            if (orders[i] != null)
                orders[i].amount = Mathf.Max(1, orders[i].amount);
        }
    }

    void EnsureDefaultOrderIfEmpty()
    {
        if (orders != null && orders.Length > 0)
            return;

        orders = new DemoOrder[]
        {
            new DemoOrder
            {
                customerName = "Ivy",
                remedyType = RemedyType.LeafInfusion,
                amount = 2
            }
        };
    }
}
