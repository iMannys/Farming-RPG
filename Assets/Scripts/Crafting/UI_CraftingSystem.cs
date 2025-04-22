using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class UI_CraftingSystem : MonoBehaviour
{
    public static UI_CraftingSystem Instance { get; private set; }

    private Transform[,] slotTransformArray;
    private CraftingResultSlot resultSlot;
    private CraftingSystem craftingSystem;

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }

        Transform craftingUI = transform.Find("Canvas").Find("Crafting");
        Transform gridContainer = craftingUI.Find("CraftingPanel").Find("CraftingSlots");
        Transform resultTransform = craftingUI.Find("CraftingPanel").Find("ResultSlot");

        resultSlot = resultTransform.GetComponentInChildren<CraftingResultSlot>();
        resultSlot.AssignIndex(-1);

        slotTransformArray = new Transform[CraftingSystem.grid_size, CraftingSystem.grid_size];

        for (int x = 0; x < CraftingSystem.grid_size; x++)
        {
            for (int y = 0; y < CraftingSystem.grid_size; y++)
            {
                slotTransformArray[x, y] = gridContainer.Find("grid_" + x + "_" + y);
                CraftingSlot craftingItemSlot = slotTransformArray[x, y].GetComponentInChildren<CraftingSlot>();
                craftingItemSlot.SetXY(x, y);
            }
        }

        for (int i = 0; i < gridContainer.childCount; i++)
        {
            CraftingSlot slot = gridContainer.GetChild(i).GetComponentInChildren<CraftingSlot>();
            slot.AssignIndex(i);
        }
    }

    public void SetCraftingSystem(CraftingSystem craftingSystem)
    {
        this.craftingSystem = craftingSystem;

        UpdateVisual();
    }

    public ItemSlotData SetGridSlot(ItemSlotData item, int x, int y)
    {
        ItemSlotData currentItem = craftingSystem.GetItem(x, y);

        if (currentItem == null)
        {
            currentItem = null;
        } else
        {
            currentItem = new ItemSlotData(currentItem);
        }

        craftingSystem.SetItem(item, x, y);

        UpdateVisual();
        return currentItem;
    }

    public bool TrySetGridSlot(ItemSlotData item, int x, int y)
    {
        if (craftingSystem.GetItem(x, y) == null)
        {
            craftingSystem.SetItem(item, x, y);
            UpdateVisual();
            return true;
        }
        return false;
    }

    public void SwitchGridSlots(int fromX, int fromY, int toX, int toY)
    {
        ItemSlotData fromItem = craftingSystem.GetItem(fromX, fromY);
        ItemSlotData toItem = craftingSystem.GetItem(toX, toY);

        craftingSystem.SetItem(fromItem, toX, toY);
        craftingSystem.SetItem(toItem, fromX, fromY);

        UpdateVisual();
    }

    public ItemSlotData GetOutputItem()
    {
        return craftingSystem.GetOutputItem();
    }

    public void ConsumeRecipeItems()
    {
        craftingSystem.ConsumeRecipeItems();
        UpdateVisual();
    }

    public void ClearTableToInventory()
    {
        for (int x = 0; x < CraftingSystem.grid_size; x++)
        {
            for (int y = 0; y < CraftingSystem.grid_size; y++)
            {
                ItemSlotData item = craftingSystem.GetItem(x, y);
                InventoryManager.Instance.ItemToInventory(InventorySlot.InventoryType.Item, item);

                craftingSystem.RemoveItem(x, y);
            }
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Cycle through grid and spawn items
        for (int x = 0; x < CraftingSystem.grid_size; x++)
        {
            for (int y = 0; y < CraftingSystem.grid_size; y++)
            {
                CraftingSlot slot = slotTransformArray[x, y].GetComponentInChildren<CraftingSlot>();
                if (!craftingSystem.IsEmpty(x, y))
                {
                    slot.Display(craftingSystem.GetItem(x, y));
                } else
                {
                    slot.Display(null);
                }
            }
        }

        craftingSystem.CreateOutput();
        ItemSlotData output = craftingSystem.GetOutputItem();
        resultSlot.Display(output);
    }
}