using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnaceRecipe
{
    public string InputItemName { get; }
    public string OutputItemName { get; }
    public float CompletionTimeSeconds { get; }
    public int AmountRequired { get; }
    public int RewardXP { get; }

    public FurnaceRecipe(string inputItemName, string outputItemName, float completionTimeSeconds, int amountRequired, int rewardXP)
    {
        InputItemName = inputItemName;
        OutputItemName = outputItemName;
        CompletionTimeSeconds = completionTimeSeconds;
        AmountRequired = amountRequired;
        RewardXP = rewardXP;
    }
}

public class FurnaceSystem
{
    private readonly string[] fuelItems;

    private readonly FurnaceRecipe[] recipeDictionary;

    private ItemSlotData inputItem;

    private FurnaceRecipe currentRecipe;

    private ItemSlotData outputItem;

    private ItemSlotData fuelItem;

    public FurnaceSystem() 
    {
        recipeDictionary = new FurnaceRecipe[] // List of recipe items
        {
            new("Wheat", "Bread", 30, 3, 50),
            new("Pumpkin", "Bowl of Soup", 5, 1, 100)
        };
        fuelItems = new string[] // List of fuel items
        {
            "Wood",
            "Egg"
        };
    }

    public bool IsInputEmpty()
    {
        return inputItem == null;
    }

    public bool IsItemFuel(ItemSlotData item)
    {
        if (item != null)
        {
            string name = item.itemData.itemName;

            foreach (string fuelName in fuelItems)
            {
                if (name == fuelName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public ItemSlotData GetInputItem()
    {
        return inputItem;
    }

    public void SetInputItem(ItemSlotData item)
    {
        inputItem = item;
    }

    private void DecreaseInputItemAmount(int amount)
    {
        ItemSlotData item = GetInputItem();
        if (item == null) return;

        item.quantity -= amount;

        if (item.quantity <= 0)
        {
            inputItem = null;
            CreateOutput();
        }
    }

    public void RemoveInputItem()
    {
        SetInputItem(null);
    }

    public ItemSlotData GetFuelItem()
    {
        return fuelItem;
    }

    public void SetFuelItem(ItemSlotData item)
    {
        fuelItem = item;
    }

    private void DecreaseFuelItemAmount()
    {
        ItemSlotData item = GetFuelItem();
        if (item == null) return;

        item.quantity--;

        if (item.quantity <= 0)
        {
            fuelItem = null;
            CreateOutput();
        }
    }

    public FurnaceRecipe GetRecipe()
    {
        if (!IsInputEmpty())
        {
            string inputItemName = inputItem.itemData.itemName;

            foreach (FurnaceRecipe furnaceRecipe in recipeDictionary)
            {
                if (furnaceRecipe.InputItemName == inputItemName)
                {
                    currentRecipe = furnaceRecipe;
                    return furnaceRecipe;
                }
            }
        }
        return null;
    }

    public bool FurnaceHasResources()
    {
        if (currentRecipe != null)
        {
            if (currentRecipe.InputItemName == inputItem.itemData.itemName && fuelItem != null)
            {
                if (inputItem.quantity >= currentRecipe.AmountRequired)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Handles input and fuel
    public void ConsumeResources()
    {
        if (GetFuelItem() == null) return; // No fuel to consume

        FurnaceRecipe recipe = GetRecipe();
        if (recipe == null) return;

        DecreaseInputItemAmount(recipe.AmountRequired);
        DecreaseFuelItemAmount();

        Skill cookingSkill = SkillManager.Instance.GetSkill(SkillType.Cooking);

        SkillManager.Instance.AddExperience(cookingSkill, recipe.RewardXP);
    }

    public void CreateOutput()
    {
        FurnaceRecipe recipeOutput = GetRecipe();
        if (recipeOutput == null)
        {
            currentRecipe = null;
        } else
        {
            currentRecipe = recipeOutput;
            ConsumeResources();
            if (outputItem != null && recipeOutput.OutputItemName == outputItem.itemData.itemName)
            {
                outputItem.quantity++;
            } else
            {
                ItemData item = Resources.Load<ItemData>("Items/" + recipeOutput.OutputItemName);
                outputItem = new ItemSlotData(item);
            }
            Skill skill = SkillManager.Instance.GetSkill(SkillType.Cooking);
            SkillManager.Instance.AddExperience(skill, outputItem.itemData.cost / 10);
        }
    }

    public ItemSlotData GetOutputItem()
    {
        return outputItem;
    }

    public void SetOutputItem(ItemSlotData item)
    {
        outputItem = item;
    }
    
}
