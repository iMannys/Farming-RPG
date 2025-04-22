using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingRecipeResult
{
    public string outputItem;
    public int rewardXP;

    public CraftingRecipeResult(string outputItem, int rewardXP)
    {
        this.outputItem = outputItem;
        this.rewardXP = rewardXP;
    }

    public override bool Equals(object obj)
    {
        if (obj is CraftingRecipeResult other)
        {
            return this.outputItem == other.outputItem && this.rewardXP == other.rewardXP;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (outputItem, rewardXP).GetHashCode();
    }
}

public class CraftingSystem
{
    public const int grid_size = 3;

    private ItemSlotData[,] itemArray;

    private Dictionary<CraftingRecipeResult, string[,]> recipeDictionary;

    private CraftingRecipeResult outputRecipeResult;
    private ItemSlotData outputItem;

    public CraftingSystem() 
    {
        itemArray = new ItemSlotData[grid_size, grid_size];

        recipeDictionary = new Dictionary<CraftingRecipeResult, string[,]>();

        // RecipeNameHere
        string[,] recipe = new string[grid_size, grid_size];
        recipe[0, 0] = "Egg"; recipe[1, 0] = string.Empty; recipe[2, 0] = string.Empty;
        recipe[0, 1] = string.Empty; recipe[1, 1] = string.Empty; recipe[2, 1] = string.Empty;
        recipe[0, 2] = string.Empty; recipe[1, 2] = string.Empty; recipe[2, 2] = string.Empty;
        recipeDictionary[new CraftingRecipeResult("Pumpkin", 15)] = recipe;

        // RecipeNameHere
        recipe = new string[grid_size, grid_size];
        recipe[0, 0] = "Pumpkin"; recipe[1, 0] = string.Empty; recipe[2, 0] = string.Empty;
        recipe[0, 1] = "Egg"; recipe[1, 1] = string.Empty; recipe[2, 1] = string.Empty;
        recipe[0, 2] = string.Empty; recipe[1, 2] = string.Empty; recipe[2, 2] = string.Empty;
        recipeDictionary[new CraftingRecipeResult("Sandwich", 25)] = recipe;

    }

    public bool IsEmpty(int x, int y)
    {
        return itemArray[x, y] == null;
    }

    public ItemSlotData GetItem(int x, int y)
    {
        return itemArray[x, y];
    }

    public void SetItem(ItemSlotData item, int x, int y)
    {
        itemArray[x, y] = item;
    }

    private void IncreaseItemAmount(int x, int y)
    {
        GetItem(x, y).quantity++;
    }

    private void DecreaseItemAmount(int x, int y)
    {
        ItemSlotData item = GetItem(x, y);
        if (item == null) return;

        item.quantity--;

        if (item.quantity <= 0)
        {
            itemArray[x, y] = null;
            CreateOutput();
        }
    }

    public void RemoveItem(int x, int y)
    {
        SetItem(null, x, y);
    }
    
    public CraftingRecipeResult GetRecipeOutput()
    {
        foreach (CraftingRecipeResult result in recipeDictionary.Keys)
        {
            string[,] recipe = recipeDictionary[result];

            bool completeRecipe = true;
            for (int x = 0; x < grid_size; x++)
            {
                for (int y = 0; y < grid_size; y++)
                {
                    if (!string.IsNullOrEmpty(recipe[x, y]))
                    {
                        ItemSlotData item = GetItem(x, y);

                        if (IsEmpty(x, y) || item.itemData == null || item.itemData.itemName != recipe[x, y])
                        {
                            completeRecipe = false;
                            break;
                        }
                    }
                    else // Slot should be empty
                    {
                        if (!IsEmpty(x, y)) // If it's not empty, the recipe is invalid
                        {
                            completeRecipe = false;
                            break;
                        }
                    }
                }
                if (!completeRecipe) break;
            }

            if (completeRecipe)
            {
                return result;//Resources.Load<ItemData>("Items/" + result.outputItem);
            }
        }
        return null;
    }

    public void ConsumeRecipeItems()
    {
        for (int x = 0; x < grid_size; x++)
        {
            for (int y = 0; y < grid_size; y++)
            {
                DecreaseItemAmount(x, y);
            }
        }
        Skill craftingSkill = SkillManager.Instance.GetSkill(SkillType.Crafting);
        SkillManager.Instance.AddExperience(craftingSkill, outputRecipeResult.rewardXP);
    }

    public void CreateOutput()
    {
        CraftingRecipeResult recipeOutput = GetRecipeOutput();
        if (recipeOutput == null)
        {
            outputItem = null;
        } else
        {
            outputRecipeResult = recipeOutput;
            ItemData item = Resources.Load<ItemData>("Items/" + recipeOutput.outputItem);
            outputItem = new ItemSlotData(item);
        }
    }

    public ItemSlotData GetOutputItem()
    {
        return outputItem;
    }
}
