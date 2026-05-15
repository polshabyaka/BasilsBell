using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpleRewardPopup : MonoBehaviour
{
    [SerializeField] GameObject popupRoot;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] Button closeButton;
    [SerializeField] float autoCloseSeconds = 1.5f;

    Coroutine autoCloseRoutine;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        Close();
    }

    public void Show(string itemName)
    {
        Show("You got", itemName);
    }

    public void Show(string title, string body)
    {
        if (titleText != null)
            titleText.text = title;

        if (bodyText != null)
            bodyText.text = body;

        if (popupRoot != null)
            popupRoot.SetActive(true);

        if (autoCloseRoutine != null)
            StopCoroutine(autoCloseRoutine);

        if (autoCloseSeconds > 0f)
            autoCloseRoutine = StartCoroutine(AutoCloseAfterDelay());
    }

    public void Close()
    {
        if (autoCloseRoutine != null)
        {
            StopCoroutine(autoCloseRoutine);
            autoCloseRoutine = null;
        }

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }

    IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(autoCloseSeconds);
        autoCloseRoutine = null;
        Close();
    }
}
