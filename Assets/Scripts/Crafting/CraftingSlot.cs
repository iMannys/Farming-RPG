using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : InventorySlot
{
    public int x;
    public int y;

    public void SetXY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override void StartDrag()
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
    
    public override void EndDrag(InventorySlot selectedSlot)
    {
        UIManager.Instance.IsDragging = false;
        UIManager.Instance.DragIndex = 0;
        UIManager.Instance.Type = null;
        UIManager.Instance.Slot = null;
        UIManager.Instance.IsMainInventory = false;
        UIManager.Instance.IsCraftingInventory = false;
        UIManager.Instance.IsFurnaceInventory = false;
        // Transform is a bit weird when disabled and enabled
        originalParent = UIManager.Instance.ItemSlotParent.transform; 
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalSiblingIndex);
        canvasGroup.alpha = 1f;
        //canvasGroup.blocksRaycasts = true;

        UIManager.Instance.ItemSlotParent = null;

        InventoryManager inventoryInstance = InventoryManager.Instance;
        UI_CraftingSystem craftingInstance = UI_CraftingSystem.Instance;


        if (selectedSlot != null)
        {
            if (selectedSlot is CraftingSlot craftingSlot)
            {
                if (slotIndex != craftingSlot.slotIndex)
                {
                    // Set positions to default
                    rectTransform.anchoredPosition = new Vector2(0, 0);

                    // Move from own xy to selectedXY
                    craftingInstance.SwitchGridSlots(x, y, craftingSlot.x, craftingSlot.y);
                }
            }
            else if (selectedSlot is HandInventorySlot)
            {
                // Set the handSlot and retrieve the item that was there before
                ItemSlotData handItem = inventoryInstance.ItemToHand(inventoryType, new ItemSlotData(itemToDisplay, quantity));

                // Set that item to this position
                craftingInstance.SetGridSlot(handItem, x, y);
            } 
            else
            {
                int selectedIndex = selectedSlot.slotIndex;

                ItemSlotData item = inventoryInstance.ItemToInventory(inventoryType, new ItemSlotData(itemToDisplay, quantity), selectedIndex);

                craftingInstance.SetGridSlot(item, x, y);
            }
            UIManager.Instance.RenderCraftingInventory();
            // Set position to default
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        rectTransform.anchoredPosition = new Vector2(0, 0);
        //eventData.pointerDrag.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
}
