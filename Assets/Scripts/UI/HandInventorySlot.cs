using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class HandInventorySlot : InventorySlot, IPointerClickHandler
{

    protected override void Awake()
    {
        base.Awake();
        AssignIndex(-1);
    }

    public void OnPointerClick(InventoryType inventoryType)
    {
        InventoryManager.Instance.HandToInventory(inventoryType);
    }


    
    public override void EndDrag(InventorySlot selectedSlot)
    {
        Debug.Log("OnEndDrag");
        UIManager.Instance.IsDragging = false;
        UIManager.Instance.DragIndex = 0;
        UIManager.Instance.Type = null;
        UIManager.Instance.Slot = null;
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalSiblingIndex);
        canvasGroup.alpha = 1f;
        //canvasGroup.blocksRaycasts = true;

        InventoryManager inventoryInstance = InventoryManager.Instance;
        UI_CraftingSystem craftingInstance = UI_CraftingSystem.Instance;
        UI_FurnaceSystem furnaceInstance = UI_FurnaceSystem.Instance;

        
        if (selectedSlot != null)
        {
            if (selectedSlot is CraftingSlot craftingSlot)
            {
                
                ItemSlotData item = inventoryInstance.HandToItem(inventoryType);

                ItemSlotData craftingItem = craftingInstance.SetGridSlot(item, craftingSlot.x, craftingSlot.y);
                
                //ItemSlotData emptyData = new ItemSlotData(itemToDisplay, quantity);
                //emptyData.Empty();

                inventoryInstance.ItemToHand(inventoryType, craftingItem);
            }
            else if (selectedSlot is FurnaceInputSlot)
            {
                // Set positions to default
                rectTransform.anchoredPosition = new Vector2(0, 0);

                ItemSlotData item = inventoryInstance.HandToItem(inventoryType);

                ItemSlotData retrievedItem = furnaceInstance.SetInputItem(item);

                inventoryInstance.ItemToHand(inventoryType, retrievedItem);

                furnaceInstance.TryRunSystem();
            }
            else if (selectedSlot is FurnaceFuelSlot)
            {
                ItemSlotData handItem = inventoryInstance.GetHandItem(inventoryType);
                if (furnaceInstance.IsItemFuel(handItem))
                {
                    ItemSlotData currentItem = furnaceInstance.SetFuelItem(handItem);

                    inventoryInstance.ItemToHand(inventoryType, currentItem);

                    furnaceInstance.TryRunSystem();
                }
            }
            else if (selectedSlot.slotIndex != slotIndex)
            {
                //InventorySlot inventorySlot = eventData.pointerDrag.GetComponent<InventorySlot>();

                int selectedIndex = selectedSlot.slotIndex;

                InventoryManager.Instance.HandToInventory(inventoryType, selectedIndex);

                // Set positions to default
                rectTransform.anchoredPosition = new Vector2(0, 0);
            }
        }
        rectTransform.anchoredPosition = new Vector2(0, 0);
        //eventData.pointerDrag.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0, 0);
        //UIManager.Instance.RenderCraftingInventory();
    }
}
