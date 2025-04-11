using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

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
    }

    List<ItemData> _itemIndex;

    public ItemData GetItemFromString(string name)
    {
        _itemIndex ??= Resources.LoadAll<ItemData>("").ToList();
        return _itemIndex.Find(i => i.name == name);
    }

    [Header("Tools")]
    //Tool Slots
    [SerializeField]
    public ItemSlotData[] toolSlots = new ItemSlotData[8];
    //Tool in the player's hand
    [SerializeField]
    private ItemSlotData equippedToolSlot = null;

    [Header("Items")]
    //Item Slots
    [SerializeField]
    public ItemSlotData[] itemSlots = new ItemSlotData[8];
    //Item in the player's hand
    [SerializeField]
    private ItemSlotData equippedItemSlot = null;

    //The transform for the player to hold items in the scene
    public Transform handPoint;

    //Load the inventory from a save
    public void LoadInventory(ItemSlotData[] toolSlots, ItemSlotData equippedToolSlot, ItemSlotData[] itemSlots, ItemSlotData equippedItemSlot)
    {
        this.toolSlots = toolSlots;
        this.equippedToolSlot = equippedToolSlot;
        this.itemSlots = itemSlots;
        this.equippedItemSlot = equippedItemSlot;

        //Update the changes to the UI and scene
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();
        RenderHand();
    }

    public ItemSlotData GetHandItem(InventorySlot.InventoryType inventoryType)
    {
        //The slot to equip (Tool by default)
        ItemSlotData handToEquip = equippedToolSlot;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            handToEquip = equippedItemSlot;
        }

        return handToEquip;
    }

    public ItemSlotData GetInventoryItem(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            inventoryToAlter = itemSlots;
        }

        return inventoryToAlter[slotIndex];
    }

    //Equipping

    //Handles movement of item from Inventory to Hand
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        //The slot to equip (Tool by default)
        ItemSlotData handToEquip = equippedToolSlot;
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            handToEquip = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }

        //Check if stackable
        if (handToEquip.Stackable(inventoryToAlter[slotIndex]))
        {
            ItemSlotData slotToAlter = inventoryToAlter[slotIndex];

            //Add to the hand slot
            handToEquip.AddQuantity(slotToAlter.quantity);

            //Empty the inventory slot
            slotToAlter.Empty();


        }
        else
        {
            //Not stackable
            //Cache the Inventory ItemSlotData
            ItemSlotData slotToEquip = new(inventoryToAlter[slotIndex]);

            //Change the inventory slot to the hands
            inventoryToAlter[slotIndex] = new ItemSlotData(handToEquip);

            //Check if the slot to equip is empty
            if (slotToEquip.IsEmpty())
            {
                //Empty the hand instead
                handToEquip.Empty();
            }
            else
            {
                EquipHandSlot(slotToEquip);
            }

        }

        //Update the changes in the scene
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();
    }

    //Handles movement of item from Hand to Inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType, int slotIndex = -1)
    {
        //The slot to move from (Tool by default)
        ItemSlotData handSlot = equippedToolSlot;
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            handSlot = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }

        if (slotIndex >= 0)
        {
            ItemSlotData inventorySlot = inventoryToAlter[slotIndex];

            // If the selected inventory slot can be stacked with the hand slot
            if (inventorySlot != null && inventorySlot.Stackable(handSlot))
            {
                //Add to the new slot
                inventoryToAlter[slotIndex].AddQuantity(handSlot.quantity);

                //Empty the inventory slot
                handSlot.Empty();
            }
            else if (inventorySlot == null && handSlot != null || inventorySlot.IsEmpty() && !handSlot.IsEmpty())
            {
                inventoryToAlter[slotIndex] = new ItemSlotData(handSlot);

                handSlot.Empty();
            }
            else
            {
                //Cache the Inventory ItemSlotData
                ItemSlotData slotDataCached = new(inventorySlot);

                inventoryToAlter[slotIndex] = handSlot;

                if (inventoryType == InventorySlot.InventoryType.Item)
                {
                    equippedItemSlot = slotDataCached;
                }
                else
                {
                    equippedToolSlot = slotDataCached;
                }
            }
        }
        else
        {
            //Try stacking the hand slot.
            //Check if the operation failed
            if (!StackItemToInventory(handSlot, inventoryToAlter))
            {
                //Find an empty slot to put the item in
                //Iterate through each inventory slot and find an empty slot
                for (int i = 0; i < inventoryToAlter.Length; i++)
                {
                    if (inventoryToAlter[i].IsEmpty())
                    {
                        //Send the equipped item over to its new slot
                        inventoryToAlter[i] = new ItemSlotData(handSlot);
                        //Remove the item from the hand
                        handSlot.Empty();
                        break;
                    }
                }

            }
        }

        //Update the changes in the scene
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();
    }

    public ItemSlotData ItemToHand(InventorySlot.InventoryType inventoryType, ItemSlotData item)
    {
        //The slot to equip (Tool by default)
        ItemSlotData handToEquip = equippedToolSlot;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            handToEquip = equippedItemSlot;
        }
        if (item == null) return null;

        //Check if stackable
        if (handToEquip != null && handToEquip.Stackable(item))
        {
            //Add to the hand slot
            handToEquip.AddQuantity(item.quantity);
        }
        else
        {
            //Not stackable

            //Check if the slot to equip is empty
            if (item != null && item.IsEmpty())
            {
                //Empty the hand instead
                handToEquip.Empty();
            }
            else
            {
                EquipHandSlot(item.itemData);
            }

        }

        ItemSlotData cachedHand = new(handToEquip);

        //Update the changes in the scene
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();

        return cachedHand;
    }

    public ItemSlotData HandToItem(InventorySlot.InventoryType inventoryType)
    {
        //The slot to equip (Tool by default)
        ItemSlotData handToEquip = equippedToolSlot;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            handToEquip = equippedItemSlot;
        }

        ItemSlotData item = new(handToEquip);
        if (item.IsEmpty()) item = null;

        handToEquip.Empty();

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();

        return item;
    }

    public ItemSlotData InventoryToItem(InventorySlot.InventoryType inventoryType, int slotIndex)
    {
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            inventoryToAlter = itemSlots;
        }

        //Cache the Inventory ItemSlotData
        ItemSlotData slotData = new(inventoryToAlter[slotIndex]);
        if (slotData.IsEmpty()) slotData = null;

        inventoryToAlter[slotIndex].Empty();

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();

        return slotData;
    }

    public ItemSlotData ItemToInventory(InventorySlot.InventoryType inventoryType, ItemSlotData item, int slotIndex = -1)
    {   
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            inventoryToAlter = itemSlots;
        }

        ItemSlotData cachedSlot = null;
        
        if (slotIndex >= 0)
        {
            ItemSlotData slot = inventoryToAlter[slotIndex];
            cachedSlot = slot != null ? new ItemSlotData(slot) : new ItemSlotData(null, 0);
            if (cachedSlot.IsEmpty()) cachedSlot = null;
            
            //Check if stackable
            if (slot != null && slot.Stackable(item))
            {
                //Add to the hand slot
                inventoryToAlter[slotIndex].AddQuantity(item.quantity);
            }
            else
            {
                //Not stackable
                //Check if the slot to equip is empty
                if (item != null && item.IsEmpty())
                {
                    //Empty the hand instead
                    inventoryToAlter[slotIndex].Empty();
                }
                else
                {
                    inventoryToAlter[slotIndex] = item;
                }
            }
        } else
        {
            bool isStacked = false;
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i] != null && inventoryToAlter[i].Stackable(item))
                {
                    cachedSlot = new ItemSlotData(inventoryToAlter[i]);
                    if (cachedSlot.IsEmpty()) cachedSlot = null;

                    //Add to the inventory slot's stack
                    inventoryToAlter[i].AddQuantity(item.quantity);

                    //Empty the item slot
                    item.Empty();
                    isStacked = true;
                    break;
                }
            }

            if (!isStacked)
            {
                //Find an empty slot to put the item in
                //Iterate through each inventory slot and find an empty slot
                for (int i = 0; i < inventoryToAlter.Length; i++)
                {
                    if (inventoryToAlter[i] == null || inventoryToAlter[i].IsEmpty()) // No need to set cachedSlot since we check if its empty
                    {
                        // Send the item to the slot
                        inventoryToAlter[i] = item;
                        break;
                    }
                }
            }
        }

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();

        return cachedSlot;
    }

    public (ItemSlotData oldItemData, ItemSlotData newItemData) MoveItem(int slotIndex, int newSlotIndex, InventorySlot.InventoryType inventoryType)
    {
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            inventoryToAlter = itemSlots;
        }

        ItemSlotData slotData = inventoryToAlter[slotIndex];
        ItemSlotData newSlotData = inventoryToAlter[newSlotIndex];

        //Check if stackable
        if (slotData.Stackable(newSlotData))
        {

            //Add to the new slot
            inventoryToAlter[newSlotIndex].AddQuantity(slotData.quantity);

            //Empty the inventory slot
            inventoryToAlter[slotIndex].Empty();


        }
        else
        {
            //Not stackable
            //Cache the Inventory ItemSlotData
            ItemSlotData slotDataCached = new(slotData);

            inventoryToAlter[slotIndex] = newSlotData;
            inventoryToAlter[newSlotIndex] = slotDataCached;

            //slotData.Empty();
        }

        return (inventoryToAlter[slotIndex], inventoryToAlter[newSlotIndex]);
    }

    //Iterate through each of the items in the inventory to see if it can be stacked
    //Will perform the operation if found, returns false if unsuccessful
    public bool StackItemToInventory(ItemSlotData itemSlot, ItemSlotData[] inventoryArray)
    {

        for (int i = 0; i < inventoryArray.Length; i++)
        {
            if (inventoryArray[i].Stackable(itemSlot))
            {
                //Add to the inventory slot's stack
                inventoryArray[i].AddQuantity(itemSlot.quantity);
                //Empty the item slot
                itemSlot.Empty();
                return true;
            }
        }

        //Can't find any slot that can be stacked
        return false;
    }

    //Handles movement of item from Shop to Inventory
    public void ShopToInventory(ItemSlotData itemSlotToMove)
    {
        //The inventory array to change
        ItemSlotData[] inventoryToAlter = IsTool(itemSlotToMove.itemData) ? toolSlots : itemSlots;

        //Try stacking the hand slot.
        //Check if the operation failed
        if (!StackItemToInventory(itemSlotToMove, inventoryToAlter))
        {
            //Find an empty slot to put the item in
            //Iterate through each inventory slot and find an empty slot
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    //Send the equipped item over to its new slot
                    inventoryToAlter[i] = new ItemSlotData(itemSlotToMove);
                    break;
                }
            }

        }

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();
        RenderHand();
    }
    //Render the player's equipped item in the scene
    public void RenderHand()
    {
        //Reset objects on the hand
        if (handPoint.childCount > 0)
        {
            Destroy(handPoint.GetChild(0).gameObject);
        }

        //Check if the player has anything equipped
        if (SlotEquipped(InventorySlot.InventoryType.Item))
        {
            //Instantiate the game model on the player's hand and put it on the scene
            Instantiate(GetEquippedSlotItem(InventorySlot.InventoryType.Item).gameModel, handPoint);
        }

    }

    //Inventory Slot Data
    #region Gets and Checks
    //Get the slot item (ItemData)
    public ItemData GetEquippedSlotItem(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot.itemData;
        }
        return equippedToolSlot.itemData;
    }

    //Get function for the slots (ItemSlotData)
    public ItemSlotData GetEquippedSlot(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot;
        }
        return equippedToolSlot;
    }

    //Get function for the inventory slots
    public ItemSlotData[] GetInventorySlots(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return itemSlots;
        }
        return toolSlots;
    }

    //Check if a hand slot has an item
    public bool SlotEquipped(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return !equippedItemSlot.IsEmpty();
        }
        return !equippedToolSlot.IsEmpty();
    }

    //Check if the item is a tool
    public bool IsTool(ItemData item)
    {
        //Is it equipment?
        //Try to cast it as equipment first
        EquipmentData equipment = item as EquipmentData;
        if (equipment != null)
        {
            return true;
        }

        //Is it a seed?
        //Try to cast it as equipment first
        SeedData seed = item as SeedData;
        //If the seed is not null it is a seed
        return seed != null;

    }

    #endregion

    //Equip the hand slot with an ItemData (Will overwrite the slot)
    public void EquipHandSlot(ItemData item)
    {
        if (IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(item);
        }
        else
        {
            equippedItemSlot = new ItemSlotData(item);
        }

    }

    //Equip the hand slot with an ItemSlotData (Will overwrite the slot)
    public void EquipHandSlot(ItemSlotData itemSlot)
    {
        //Get the item data from the slot
        ItemData item = itemSlot.itemData;

        if (IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(itemSlot);
        }
        else
        {
            equippedItemSlot = new ItemSlotData(itemSlot);
        }
    }

    public void ConsumeItem(ItemSlotData itemSlot)
    {
        if (itemSlot.IsEmpty())
        {
            Debug.LogError("There is nothing to consume!");
            return;
        }

        //Use up one of the item slots
        itemSlot.Remove();
        //Refresh inventory
        RenderHand();
        UIManager.Instance.RenderInventory();
        UIManager.Instance.RenderCraftingInventory();
        UIManager.Instance.RenderFurnaceInventory();
    }


    #region Inventory Slot Validation
    private void OnValidate()
    {
        //Validate the hand slots
        ValidateInventorySlot(equippedToolSlot);
        ValidateInventorySlot(equippedItemSlot);

        //Validate the slots in the inventory
        ValidateInventorySlots(itemSlots);
        ValidateInventorySlots(toolSlots);

    }

    //When giving the itemData value in the inspector, automatically set the quantity to 1
    void ValidateInventorySlot(ItemSlotData slot)
    {
        if (slot.itemData != null && slot.quantity == 0)
        {
            slot.quantity = 1;
        }
    }

    //Validate arrays
    void ValidateInventorySlots(ItemSlotData[] array)
    {
        foreach (ItemSlotData slot in array)
        {
            ValidateInventorySlot(slot);
        }
    }
    #endregion
}
