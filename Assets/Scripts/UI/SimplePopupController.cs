using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimplePopupController : MonoBehaviour
{
    [Header("Popup Roots")]
    [SerializeField] GameObject popupLayer;
    [SerializeField] GameObject dimBackground;
    [SerializeField] GameObject smallConfirmWindow;
    [SerializeField] GameObject largeWorkWindow;

    [Header("Small Confirm Texts")]
    [SerializeField] TMP_Text confirmTitleText;
    [SerializeField] TMP_Text confirmBodyText;

    [Header("Large Window")]
    [SerializeField] TMP_Text modalTitleText;
    [SerializeField] GameObject orderContent;
    [SerializeField] GameObject cauldronContent;
    [SerializeField] GameObject shelfContent;

    [Header("Shop Interactable Buttons")]
    [SerializeField] Button doorButton;
    [SerializeField] Button inventoryButton;
    [SerializeField] Button cauldronButton;
    [SerializeField] Button orderButton;

    [Header("Popup Buttons")]
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button closeButton;

    [Header("Flow")]
    [SerializeField] GameFlowController gameFlow;

    void Start()
    {
        if (doorButton != null)
            doorButton.onClick.AddListener(OpenForestConfirm);

        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(OpenInventory);

        if (cauldronButton != null)
            cauldronButton.onClick.AddListener(OpenCauldron);

        if (orderButton != null)
            orderButton.onClick.AddListener(OpenOrder);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmGoToForest);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Close);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        ConfigureDimBackgroundRaycasts();
        Close();
    }

    public void OpenForestConfirm()
    {
        HideAllWindows();
        HideAllLargeContents();
        OpenPopupLayer();

        if (smallConfirmWindow != null)
            smallConfirmWindow.SetActive(true);

        if (confirmTitleText != null)
            confirmTitleText.text = "Forest Door";

        if (confirmBodyText != null)
            confirmBodyText.text = "The forest door is humming softly.\n\nStep outside?";
    }

    public void OpenOrder()
    {
        HideAllWindows();
        OpenPopupLayer();

        if (largeWorkWindow != null)
            largeWorkWindow.SetActive(true);

        if (modalTitleText != null)
            modalTitleText.text = "Order";

        ShowOnlyContent(orderContent);
    }

    public void OpenCauldron()
    {
        HideAllWindows();
        OpenPopupLayer();

        if (largeWorkWindow != null)
            largeWorkWindow.SetActive(true);

        if (modalTitleText != null)
            modalTitleText.text = "Cauldron";

        ShowOnlyContent(cauldronContent);
    }

    public void OpenInventory()
    {
        HideAllWindows();
        OpenPopupLayer();

        if (largeWorkWindow != null)
            largeWorkWindow.SetActive(true);

        if (modalTitleText != null)
            modalTitleText.text = "Herb Shelf";

        ShowOnlyContent(shelfContent);
    }

    public void ConfirmGoToForest()
    {
        Close();

        if (gameFlow != null)
            gameFlow.EnterForest();
    }

    public void Close()
    {
        HideAllWindows();
        HideAllLargeContents();

        if (dimBackground != null)
            dimBackground.SetActive(false);

        if (popupLayer != null)
            popupLayer.SetActive(false);

        SetShopButtonsInteractable(true);
    }

    void OpenPopupLayer()
    {
        if (popupLayer != null)
            popupLayer.SetActive(true);

        if (dimBackground != null)
            dimBackground.SetActive(true);

        SetShopButtonsInteractable(false);
    }

    void HideAllWindows()
    {
        if (smallConfirmWindow != null)
            smallConfirmWindow.SetActive(false);

        if (largeWorkWindow != null)
            largeWorkWindow.SetActive(false);
    }

    void HideAllLargeContents()
    {
        if (orderContent != null)
            orderContent.SetActive(false);

        if (cauldronContent != null)
            cauldronContent.SetActive(false);

        if (shelfContent != null)
            shelfContent.SetActive(false);
    }

    void ShowOnlyContent(GameObject content)
    {
        HideAllLargeContents();

        if (content != null)
            content.SetActive(true);
    }

    void ConfigureDimBackgroundRaycasts()
    {
        if (dimBackground == null) return;

        Graphic graphic = dimBackground.GetComponent<Graphic>();
        if (graphic != null)
            graphic.raycastTarget = true;

        CanvasGroup canvasGroup = dimBackground.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    void SetShopButtonsInteractable(bool interactable)
    {
        if (doorButton != null)
            doorButton.interactable = interactable;

        if (inventoryButton != null)
            inventoryButton.interactable = interactable;

        if (cauldronButton != null)
            cauldronButton.interactable = interactable;

        if (orderButton != null)
            orderButton.interactable = interactable;
    }
}
