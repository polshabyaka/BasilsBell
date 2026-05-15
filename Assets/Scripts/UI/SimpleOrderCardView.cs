using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimpleOrderCardView : MonoBehaviour
{
    [SerializeField] Image paperImage;
    [SerializeField] Image remedyImage;
    [SerializeField] TMP_Text countText;
    [SerializeField] Button button;
    [SerializeField] GameObject completedRoot;

    public void SetOrder(Sprite paperSprite, Sprite remedySprite, int amount, bool completed, UnityAction onClick)
    {
        if (paperImage != null)
        {
            paperImage.sprite = paperSprite;
            paperImage.enabled = paperSprite != null;
        }

        if (remedyImage != null)
        {
            remedyImage.sprite = remedySprite;
            remedyImage.enabled = remedySprite != null;
        }

        if (countText != null)
            countText.text = "x" + amount;

        if (completedRoot != null)
            completedRoot.SetActive(completed);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();

            if (onClick != null)
                button.onClick.AddListener(onClick);

            button.interactable = !completed;
        }
    }
}
