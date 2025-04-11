using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingResultSlot : InventorySlot
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
        transform.SetParent(originalParent.parent.parent, true);
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
        UI_CraftingSystem craftingInstance = UI_CraftingSystem.Instance;

        if (selectedSlot != null)
        {
            ItemSlotData item = craftingInstance.GetOutputItem();
            craftingInstance.ConsumeRecipeItems();
            if (selectedSlot is CraftingSlot craftingSlot)
            {
                // Set positions to default
                rectTransform.anchoredPosition = new Vector2(0, 0);

                craftingInstance.TrySetGridSlot(item, craftingSlot.x, craftingSlot.y);
            }
            else if (selectedSlot is HandInventorySlot)
            {
                inventoryInstance.ItemToHand(inventoryType, item);
            }
            else
            {
                int selectedIndex = selectedSlot.slotIndex;

                inventoryInstance.ItemToInventory(inventoryType, item, selectedIndex);
            }
            Skill skill = SkillManager.Instance.GetSkill(SkillType.Crafting);
            SkillManager.Instance.AddExperience(skill, item.itemData.cost / 10);
            UIManager.Instance.RenderCraftingInventory();
            // Set position to default
            //rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        rectTransform.anchoredPosition = new Vector2(0, 0);
        //eventData.pointerDrag.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
}
