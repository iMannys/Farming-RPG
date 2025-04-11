using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnaceInputSlot : InventorySlot
{
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
        originalParent = UIManager.Instance.ItemSlotParent.transform; // Transform is a bit weird when disabled and enabled
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalSiblingIndex);
        canvasGroup.alpha = 1f;
        //canvasGroup.blocksRaycasts = true;

        UIManager.Instance.ItemSlotParent = null;

        InventoryManager inventoryInstance = InventoryManager.Instance;
        UI_FurnaceSystem furnaceInstance = UI_FurnaceSystem.Instance;

        if (selectedSlot != null)
        {
            ItemSlotData item = furnaceInstance.GetInputItem();
            if (selectedSlot is FurnaceFuelSlot)
            {
                if (furnaceInstance.IsItemFuel(item))
                {
                    ItemSlotData currentItem = furnaceInstance.SetFuelItem(item);

                    furnaceInstance.SetInputItem(currentItem);

                    furnaceInstance.TryRunSystem();
                }
            }
            else if (selectedSlot is HandInventorySlot)
            {
                inventoryInstance.ItemToHand(inventoryType, item);
            }
            else if (selectedSlot.GetType() == typeof(InventorySlot))
            {
                int selectedIndex = selectedSlot.slotIndex;

                ItemSlotData retrievedItem = inventoryInstance.ItemToInventory(inventoryType, item, selectedIndex);

                furnaceInstance.SetInputItem(retrievedItem);
            }
            // Set position to default
            //rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        rectTransform.anchoredPosition = new Vector2(0, 0);
        //eventData.pointerDrag.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
}
