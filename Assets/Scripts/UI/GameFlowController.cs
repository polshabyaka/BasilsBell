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

    [Header("Optional UI")]
    [SerializeField] TMP_Text hintText;

    [Header("Forest")]
    [SerializeField] GridManager grid;

    GameMode currentMode = GameMode.Shop;

    IEnumerator Start()
    {
        if (goToForestButton != null)
            goToForestButton.onClick.AddListener(EnterForest);

        if (returnToShopButton != null)
            returnToShopButton.onClick.AddListener(ReturnFromForest);

        HideReturnPrompt();

        // Let GridManager start first while ForestRoot is still active.
        yield return null;

        EnterShop();
    }

    void Update()
    {
        if (currentMode != GameMode.Forest) return;
        if (grid == null || grid.player == null) return;

        bool canReturn = IsPlayerOnReturnCell();

        if (returnToShopButton != null)
            returnToShopButton.gameObject.SetActive(canReturn);

        if (hintText != null)
        {
            hintText.gameObject.SetActive(canReturn);
            if (canReturn)
                hintText.text = "Return to shop";
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
        if (!IsPlayerOnReturnCell()) return;

        EnterShop();
    }

    bool IsPlayerOnReturnCell()
    {
        if (grid == null || grid.player == null) return false;

        int homeX = grid.width / 2;
        int homeY = grid.height / 2;

        return grid.player.gridX == homeX && grid.player.gridY == homeY;
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