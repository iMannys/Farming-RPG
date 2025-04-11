using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpListing : MonoBehaviour
{
    public Image messageIconImage;
    public TextMeshProUGUI messageInfoText;

    public void SetPopupMessage(Sprite icon, string message)
    {
        // Set the content
        messageIconImage.sprite = icon;
        messageInfoText.text = message;
    }
}
