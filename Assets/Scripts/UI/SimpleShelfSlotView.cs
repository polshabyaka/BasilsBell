using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleShelfSlotView : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text countText;
    [SerializeField] TMP_Text nameText;

    public void SetItem(Sprite icon, string displayName, int count)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        if (countText != null)
            countText.text = "x" + count;

        if (nameText != null)
            nameText.text = displayName;
    }
}
