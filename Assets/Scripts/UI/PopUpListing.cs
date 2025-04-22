using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpListing : MonoBehaviour
{
    public Image messageIconImage;
    public TextMeshProUGUI messageInfoText;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void SetPopupMessage(Sprite icon, string message)
    {
        // Set the content
        messageIconImage.sprite = icon;
        messageInfoText.text = message;
    }

    public void StartFadeAndDestroy(float duration, float fadeDuration)
    {
        StartCoroutine(FadeOutAndDestroy(duration, fadeDuration));
    }

    private IEnumerator FadeOutAndDestroy(float duration, float fadeDuration)
    {
        yield return new WaitForSeconds(duration - fadeDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        Destroy(gameObject);
    }
}
