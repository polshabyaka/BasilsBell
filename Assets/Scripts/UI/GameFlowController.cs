using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    public enum GameMode
    {
        Shop,
        Forest
    }

    [Header("Mode Roots")]
    [SerializeField] GameObject shopRoot;
    [SerializeField] GameObject forestRoot;

    [Header("Buttons")]
    [SerializeField] Button goToForestButton;
    [SerializeField] Button returnToShopButton;

    [Header("Start Menu")]
    [SerializeField] bool showStartMenuOnLaunch = true;
    [SerializeField] bool createStartMenuIfMissing = true;
    [SerializeField] bool showQuitButton = true;
    [SerializeField] GameObject startMenuRoot;
    [SerializeField] Button startMenuStartButton;
    [SerializeField] Button startMenuQuitButton;

    [Header("Optional UI")]
    [SerializeField] TMP_Text hintText;

    [Header("Forest")]
    [SerializeField] GridManager grid;

    GameMode currentMode = GameMode.Shop;
    bool startMenuPrepared;

    IEnumerator Start()
    {
        if (goToForestButton != null)
            goToForestButton.onClick.AddListener(EnterForest);

        if (returnToShopButton != null)
            returnToShopButton.onClick.AddListener(ReturnFromForest);

        ConfigureReturnPromptRaycasts();
        HideReturnPrompt();
        PrepareStartMenu();

        // Let GridManager start first while ForestRoot is still active.
        yield return null;

        EnterShop();

        if (showStartMenuOnLaunch)
            ShowStartMenu();
        else
            HideStartMenu();
    }

    void Update()
    {
        if (currentMode != GameMode.Forest) return;
        if (grid == null || grid.player == null) return;

        bool canReturn = IsPlayerNearReturnCell();

        if (returnToShopButton != null)
            returnToShopButton.gameObject.SetActive(canReturn);

        if (hintText != null)
        {
            hintText.gameObject.SetActive(canReturn);
        }

        if (canReturn && Input.GetKeyDown(KeyCode.T))
            ReturnFromForest();
    }

    public void EnterShop()
    {
        currentMode = GameMode.Shop;

        if (shopRoot != null)
            shopRoot.SetActive(true);

        if (forestRoot != null)
            forestRoot.SetActive(false);

        HideReturnPrompt();
        SetPlayerInputLocked(true);
    }

    public void EnterForest()
    {
        currentMode = GameMode.Forest;

        if (shopRoot != null)
            shopRoot.SetActive(false);

        if (forestRoot != null)
            forestRoot.SetActive(true);

        HideReturnPrompt();
        SetPlayerInputLocked(false);
    }

    public void ReturnFromForest()
    {
        if (currentMode != GameMode.Forest) return;
        if (!IsPlayerNearReturnCell()) return;

        if (grid != null && grid.player != null)
            grid.player.IgnoreClickToMoveThisFrame();

        EnterShop();
    }

    public void ShowStartMenu()
    {
        if (!PrepareStartMenu())
            return;

        if (startMenuRoot != null)
            startMenuRoot.SetActive(true);

        SetPlayerInputLocked(true);
    }

    public void StartDemoFromMenu()
    {
        HideStartMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    bool PrepareStartMenu()
    {
        if (startMenuPrepared)
            return startMenuRoot != null;

        startMenuPrepared = true;

        if (startMenuRoot == null && createStartMenuIfMissing)
            CreateRuntimeStartMenu();

        AutoAssignStartMenuButtons();

        if (startMenuStartButton != null)
            startMenuStartButton.onClick.AddListener(StartDemoFromMenu);

        if (startMenuQuitButton != null)
        {
            startMenuQuitButton.onClick.AddListener(QuitGame);
            startMenuQuitButton.gameObject.SetActive(showQuitButton);
        }

        HideStartMenu();
        return startMenuRoot != null;
    }

    void HideStartMenu()
    {
        if (startMenuRoot != null)
            startMenuRoot.SetActive(false);
    }

    void AutoAssignStartMenuButtons()
    {
        if (startMenuRoot == null)
            return;

        if (startMenuStartButton == null)
            startMenuStartButton = FindButtonInStartMenu("StartButton");

        if (startMenuQuitButton == null)
            startMenuQuitButton = FindButtonInStartMenu("QuitButton");
    }

    Button FindButtonInStartMenu(string objectName)
    {
        Button[] buttons = startMenuRoot.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.name == objectName)
                return buttons[i];
        }

        return null;
    }

    void CreateRuntimeStartMenu()
    {
        Canvas canvas = FindStartMenuCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("Start menu could not be created because no Canvas was found.");
            return;
        }

        startMenuRoot = CreateUiObject("StartMenu", canvas.transform);
        RectTransform rootRect = startMenuRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image background = startMenuRoot.AddComponent<Image>();
        background.color = new Color(0.08f, 0.11f, 0.09f, 0.94f);

        CanvasGroup canvasGroup = startMenuRoot.AddComponent<CanvasGroup>();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        GameObject content = CreateUiObject("StartMenuContent", startMenuRoot.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(420f, 360f);

        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 18f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateMenuText(content.transform, "TitleText", "Basil's Bell", 54f, 80f);
        CreateMenuText(content.transform, "SubtitleText", "A cozy herbalist demo", 24f, 42f);
        startMenuStartButton = CreateMenuButton(content.transform, "StartButton", "Start Day");
        startMenuQuitButton = CreateMenuButton(content.transform, "QuitButton", "Quit");
    }

    Canvas FindStartMenuCanvas()
    {
        if (shopRoot != null)
        {
            Canvas shopCanvas = shopRoot.GetComponentInParent<Canvas>();
            if (shopCanvas != null)
                return shopCanvas;
        }

        return FindFirstObjectByType<Canvas>();
    }

    GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.layer = LayerMask.NameToLayer("UI");
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    TMP_Text CreateMenuText(Transform parent, string objectName, string text, float fontSize, float height)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        TMP_Text label = textObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.enableAutoSizing = true;
        label.fontSizeMax = fontSize;
        label.fontSizeMin = 18f;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        label.color = new Color(0.94f, 0.91f, 0.82f, 1f);

        LayoutElement layout = textObject.AddComponent<LayoutElement>();
        layout.preferredHeight = height;

        return label;
    }

    Button CreateMenuButton(Transform parent, string objectName, string labelText)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.86f, 0.71f, 0.42f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.96f, 0.82f, 0.51f, 1f);
        colors.pressedColor = new Color(0.68f, 0.51f, 0.29f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(320f, 72f);

        LayoutElement buttonLayout = buttonObject.AddComponent<LayoutElement>();
        buttonLayout.preferredWidth = 320f;
        buttonLayout.preferredHeight = 72f;

        TMP_Text label = CreateMenuText(buttonObject.transform, "Text", labelText, 26f, 72f);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.color = new Color(0.12f, 0.09f, 0.06f, 1f);

        return button;
    }

    void ConfigureReturnPromptRaycasts()
    {
        if (hintText != null)
            hintText.raycastTarget = false;

        if (returnToShopButton == null) return;

        Graphic targetGraphic = returnToShopButton.targetGraphic;
        Graphic[] graphics = returnToShopButton.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] != targetGraphic)
                graphics[i].raycastTarget = false;
        }
    }
    bool IsPlayerNearReturnCell()
    {
        if (grid == null || grid.player == null) return false;

        int homeX = grid.width / 2;
        int homeY = grid.height / 2;

        int dx = Mathf.Abs(grid.player.gridX - homeX);
        int dy = Mathf.Abs(grid.player.gridY - homeY);

        return dx <= 1 && dy <= 1;
    }


    void HideReturnPrompt()
    {
        if (returnToShopButton != null)
            returnToShopButton.gameObject.SetActive(false);

        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    void SetPlayerInputLocked(bool locked)
    {
        if (grid == null || grid.player == null) return;

        grid.player.inputLocked = locked;

        if (locked)
            grid.player.ForceStop();
    }
}
