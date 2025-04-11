using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour
{
    public RawImage rawImage;
    public float maxOffset = 1.0f;

    void Update()
    {
        var timestamp = TimeManager.Instance.GetGameTimestamp();
        int hour = timestamp.hour;
        int minute = timestamp.minute;

        float timeFactor = ((hour + 12) % 24) / 24.0f + (minute / 1440.0f);

        rawImage.uvRect = new Rect(new Vector2(0, timeFactor * maxOffset), rawImage.uvRect.size);
    }
}
