using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject objectToShow; // Tooltip panel (UI)
    public TextMeshProUGUI textToShow; // Text component displaying description
    public Vector2 offset = new Vector2(20f, -20f); // Offset from the cursor

    [TextArea] public string descriptionText; // Unique description for each object

    private static HoverDisplay currentHovered; // Tracks the currently active tooltip
    private bool isHovering = false;
    private RectTransform tooltipRect; // To properly position tooltip in UI space
    private Canvas canvas; // Reference to UI canvas

    void Start()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(false); // Ensure tooltip starts hidden
            tooltipRect = objectToShow.GetComponent<RectTransform>(); 
            canvas = objectToShow.GetComponentInParent<Canvas>(); // Get canvas reference
        }
    }

    void Update()
    {
        if (isHovering && objectToShow != null && canvas != null)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 localPoint;
            
            // Convert screen position to local position in the canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, 
                mousePosition, 
                canvas.worldCamera, 
                out localPoint
            );

            tooltipRect.anchoredPosition = localPoint + offset; // Apply offset correctly
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentHovered != null && currentHovered != this)
        {
            currentHovered.HideTooltip(); // Hide previous tooltip
        }

        currentHovered = this;
        isHovering = true;

        if (objectToShow != null)
        {
            objectToShow.SetActive(true);
        }

        if (textToShow != null && textToShow.text != descriptionText)
        {
            textToShow.text = descriptionText; // Display correct description
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentHovered == this)
        {
            HideTooltip();
        }
    }

    private void HideTooltip()
    {
        isHovering = false;

        if (objectToShow != null)
        {
            objectToShow.SetActive(false);
        }

        if (textToShow != null)
        {
            textToShow.text = ""; // Clear text when hiding
        }

        currentHovered = null;
    }
}
