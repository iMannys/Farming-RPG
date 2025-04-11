using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Security;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Canvas canvas;

    public ItemData itemToDisplay;
    public int quantity;

    public Image itemDisplayImage;
    public TextMeshProUGUI quantityText;
    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;
    protected Transform originalParent;
    protected int originalSiblingIndex;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public enum InventoryType
    {
        Item, Tool
    }
    //Determines which inventory section this slot is apart of.
    public InventoryType inventoryType;

    public int slotIndex;

    public void Display(ItemSlotData itemSlot)
    {
        //By default, the quantity text should not show
        quantityText.text = "";
        
        //Check if there is an item to display
        if (itemSlot != null && itemSlot.itemData != null)
        {
            //Set the variables accordingly
            itemToDisplay = itemSlot.itemData;
            quantity = itemSlot.quantity;

            //Switch the thumbnail over
            itemDisplayImage.sprite = itemToDisplay.thumbnail;

            //Display the stack quantity if there is more than 1 in the stack
            if (quantity > 1)
            {
                quantityText.text = quantity.ToString();
            }

            itemDisplayImage.gameObject.SetActive(true);

            return;
        }


        itemDisplayImage.gameObject.SetActive(false);


    }
    /*
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //Move item from inventory to hand
        InventoryManager.Instance.InventoryToHand(slotIndex, inventoryType);
    }
    */

    //Set the Slot index
    public void AssignIndex(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }

    public void HandleDragDrop()
    {

    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        InventorySlot selectedSlot = null;

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult raycastResult in results)
        {
            if (raycastResult.gameObject.TryGetComponent<ItemSlot>(out _))
            {
                InventorySlot slot = raycastResult.gameObject.GetComponentInChildren<InventorySlot>();
                // There is an item slot but there is no slot (content) which means that it is being dragged.
                if (slot == null)
                {
                    selectedSlot = this;
                }
                else
                {
                    if (slot.inventoryType != inventoryType) continue;
                    selectedSlot = slot;
                }   
            }
        }

        if (selectedSlot != null)
        {
            if (UIManager.Instance.IsDragging)
            {
                if (selectedSlot.slotIndex != slotIndex || selectedSlot is CraftingSlot || selectedSlot is CraftingResultSlot || selectedSlot is FurnaceInputSlot || selectedSlot is FurnaceFuelSlot || selectedSlot is FurnaceResultSlot)
                {
                    EndDrag(selectedSlot);
                }
                else
                {
                    EndDrag(null);
                }
            }
            else
            {
                if (IsDescendantOf(transform, canvas.transform.Find("PlayerMenu")))
                {
                    UIManager.Instance.IsMainInventory = true;
                }
                else if (IsDescendantOf(transform, canvas.transform.Find("Crafting")))
                {
                    UIManager.Instance.IsCraftingInventory = true;
                }
                else if (IsDescendantOf(transform, canvas.transform.Find("Furnace")))
                {
                    UIManager.Instance.IsFurnaceInventory = true;
                }
                UIManager.Instance.IsDragging = true;
                UIManager.Instance.DragIndex = slotIndex;
                UIManager.Instance.Slot = selectedSlot;
                UIManager.Instance.Type = inventoryType;
                StartDrag();
            }
        }
        else
        {
            EndDrag(null);
            //eventData.pointerDrag.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }

    public virtual void StartDrag()
    {
        UIManager.Instance.ItemSlotParent = transform.parent.GetComponent<ItemSlot>();
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        // Move to the top by setting as last sibling by moving out of grid
        transform.SetParent(originalParent.parent.parent.parent, true);
        transform.SetAsLastSibling();

        canvasGroup.alpha = .6f;
        //canvasGroup.blocksRaycasts = false;
    }


    //Display the item info on the item info box when the player mouses over
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.DisplayItemInfo(itemToDisplay);
    }

    //Reset the item info box when the player leaves
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.DisplayItemInfo(null);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        
    }

    public virtual void EndDrag(InventorySlot selectedSlot)
    {
        UIManager.Instance.IsDragging = false;
        UIManager.Instance.DragIndex = 0;
        UIManager.Instance.Type = null;
        UIManager.Instance.Slot = null;
        UIManager.Instance.IsMainInventory = false;
        UIManager.Instance.IsCraftingInventory = false;
        UIManager.Instance.IsFurnaceInventory = false;
        originalParent = UIManager.Instance.ItemSlotParent.transform; // Transform is a bit weird when disabled and enabled
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalSiblingIndex);
        canvasGroup.alpha = 1f;
        //canvasGroup.blocksRaycasts = true;

        UIManager.Instance.ItemSlotParent = null;

        InventoryManager inventoryInstance = InventoryManager.Instance;
        UI_CraftingSystem craftingInstance = UI_CraftingSystem.Instance;
        UI_FurnaceSystem furnaceInstance = UI_FurnaceSystem.Instance;


        if (selectedSlot != null)
        {
            if (selectedSlot is HandInventorySlot)
            {
                InventoryManager.Instance.InventoryToHand(slotIndex, inventoryType);
            }
            else if (selectedSlot is FurnaceInputSlot)
            {
                ItemSlotData item = inventoryInstance.InventoryToItem(inventoryType, slotIndex);

                ItemSlotData currentItem = furnaceInstance.SetInputItem(item);

                inventoryInstance.ItemToInventory(inventoryType, currentItem, slotIndex);

                furnaceInstance.TryRunSystem();
            }
            else if (selectedSlot is FurnaceFuelSlot)
            {
                ItemSlotData inventoryItem = inventoryInstance.GetInventoryItem(slotIndex, inventoryType);

                if (furnaceInstance.IsItemFuel(inventoryItem))
                {
                    ItemSlotData item = inventoryInstance.InventoryToItem(inventoryType, slotIndex);

                    ItemSlotData currentItem = furnaceInstance.SetFuelItem(item);

                    inventoryInstance.ItemToInventory(inventoryType, currentItem, slotIndex);

                    furnaceInstance.TryRunSystem();
                }
            }
            else if (selectedSlot is CraftingSlot craftingSlot)
            {
                ItemSlotData item = inventoryInstance.InventoryToItem(inventoryType, slotIndex);

                ItemSlotData currentItem = craftingInstance.SetGridSlot(item, craftingSlot.x, craftingSlot.y);

                //ItemSlotData emptyData = new ItemSlotData(itemToDisplay, quantity);
                //emptyData.Empty();

                inventoryInstance.ItemToInventory(inventoryType, currentItem, slotIndex);

                //Display(currentItem);
            }
            else if (selectedSlot.slotIndex != slotIndex)
            {
                //InventorySlot inventorySlot = eventData.pointerDrag.GetComponent<InventorySlot>();
                int selectedIndex = selectedSlot.slotIndex;

                var (oldItemData, newItemData) = inventoryInstance.MoveItem(slotIndex, selectedIndex, inventoryType);

                itemToDisplay = oldItemData.itemData;
                selectedSlot.itemToDisplay = newItemData.itemData;
                quantity = oldItemData.quantity;
                selectedSlot.quantity = newItemData.quantity;


                selectedSlot.Display(newItemData);
                Display(oldItemData);

                // Set positions to default
                rectTransform.anchoredPosition = new Vector2(0, 0);
            }
            // Set position to default
            selectedSlot.rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        rectTransform.anchoredPosition = new Vector2(0, 0);
        //eventData.pointerDrag.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
    }

    public void Update()
    {
        if (UIManager.Instance.Slot)
        {
            if (UIManager.Instance.Slot.GetType() != this.GetType()) return;
        }

        if (UIManager.Instance.IsDragging && UIManager.Instance.DragIndex == slotIndex
            && UIManager.Instance.Type == inventoryType)
        {
            if (UIManager.Instance.IsMainInventory
                && IsDescendantOf(transform, canvas.transform.Find("PlayerMenu")) == false) return;
            if (UIManager.Instance.IsCraftingInventory
                && IsDescendantOf(transform, canvas.transform.Find("Crafting")) == false) return;
            if (UIManager.Instance.IsFurnaceInventory
                && IsDescendantOf(transform, canvas.transform.Find("Furnace")) == false) return;

            Vector2 dragOffset = new Vector2(0, 0);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform,
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localMousePosition
            );

            rectTransform.anchoredPosition = localMousePosition + dragOffset;
        }
    }

    public virtual void OnEnable()
    {
        if (UIManager.Instance.Slot && UIManager.Instance.IsDragging)
        {
            if (UIManager.Instance.IsMainInventory 
                && IsDescendantOf(transform, canvas.transform.Find("PlayerMenu")) == false) return;
            if (UIManager.Instance.IsCraftingInventory
                && IsDescendantOf(transform, canvas.transform.Find("Crafting")) == false) return;
            if (UIManager.Instance.IsFurnaceInventory
                && IsDescendantOf(transform, canvas.transform.Find("Furnace")) == false) return;
            
            if (UIManager.Instance.Slot.GetType() != this.GetType()) return;
            if (UIManager.Instance.Type != inventoryType) return;
            if (UIManager.Instance.DragIndex != slotIndex) return;
            EndDrag(null);
        }
    }

    public static bool IsDescendantOf(Transform child, Transform potentialAncestor)
    {
        Transform current = child.parent;
        while (current != null)
        {
            if (current == potentialAncestor)
                return true;
            current = current.parent;
        }
        return false;
    }
}
