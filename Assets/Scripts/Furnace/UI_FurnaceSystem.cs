using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class UI_FurnaceSystem : MonoBehaviour
{
    public static UI_FurnaceSystem Instance { get; private set; }

    private Transform[,] slotTransformArray;
    private FurnaceInputSlot inputSlot;
    private FurnaceFuelSlot fuelSlot;
    private FurnaceResultSlot resultSlot;
    private RectTransform progressBar;
    private TextMeshProUGUI progressText;
    private FurnaceSystem furnaceSystem;

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

        Transform furnacePanelUI = transform.Find("Canvas").Find("Furnace").Find("FurnacePanel");
        Transform inputGridTransform = furnacePanelUI.Find("FurnaceSlot");
        Transform resultTransform = furnacePanelUI.Find("ResultSlot");
        progressText = furnacePanelUI.Find("Countdown").GetComponent<TextMeshProUGUI>();

        foreach (Transform child in inputGridTransform)
        {
            FurnaceInputSlot input = child.GetComponentInChildren<FurnaceInputSlot>();
            if (input)
            {
                inputSlot = input;
            } else
            {
                fuelSlot = child.GetComponentInChildren<FurnaceFuelSlot>();
            }
        }

        resultSlot = resultTransform.GetComponentInChildren<FurnaceResultSlot>();

        // So the index doesnt interfere with the other slot systems
        inputSlot.AssignIndex(-1);
        fuelSlot.AssignIndex(-1);
        resultSlot.AssignIndex(-1);

        progressBar = furnacePanelUI.Find("ProgressBarBackground").Find("ProgressBar").GetComponent<RectTransform>();

        //CreateItem(0, 0, new Item { itemType = Item.ItemType.Diamond });
        //CreateItem(1, 2, new Item { itemType = Item.ItemType.Wood });
        //CreateItemOutput(new Item { itemType = Item.ItemType.Sword_Wood });
    }

    public void SetFurnaceSystem(FurnaceSystem furnaceSystem)
    {
        this.furnaceSystem = furnaceSystem;

        UpdateVisual();
    }

    public ItemSlotData SetInputItem(ItemSlotData item)
    {
        ItemSlotData currentItem = furnaceSystem.GetInputItem();

        if (currentItem == null)
        {
            currentItem = null;
        } else
        {
            currentItem = new ItemSlotData(currentItem);
        }

        furnaceSystem.SetInputItem(item);

        UpdateVisual();
        return currentItem;
    }

    public bool TrySetInputItem(ItemSlotData item)
    {
        if (furnaceSystem.GetInputItem() == null)
        {
            furnaceSystem.SetInputItem(item);
            UpdateVisual();
            return true;
        }
        return false;
    }

    public ItemSlotData SetFuelItem(ItemSlotData item)
    {
        ItemSlotData currentItem = furnaceSystem.GetFuelItem();

        if (currentItem == null)
        {
            currentItem = null;
        }
        else
        {
            currentItem = new ItemSlotData(currentItem);
        }

        furnaceSystem.SetFuelItem(item);

        UpdateVisual();
        return currentItem;
    }

    public ItemSlotData SetOutputItem(ItemSlotData item)
    {
        ItemSlotData currentItem = furnaceSystem.GetOutputItem();

        if (currentItem == null)
        {
            currentItem = null;
        }
        else
        {
            currentItem = new ItemSlotData(currentItem);
        }

        furnaceSystem.SetOutputItem(item);

        UpdateVisual();
        return currentItem;
    }

    public ItemSlotData GetInputItem()
    {
        return furnaceSystem.GetInputItem();
    }

    public ItemSlotData GetFuelItem()
    {
        return furnaceSystem.GetFuelItem();
    }

    public ItemSlotData GetOutputItem()
    {
        return furnaceSystem.GetOutputItem();
    }

    public bool IsItemFuel(ItemSlotData item)
    {
        return furnaceSystem.IsItemFuel(item);
    }

    public void TryRunSystem()
    {
        StartCoroutine(ConsumeRecipeItems());
    }

    public IEnumerator ConsumeRecipeItems()
    {
        while (true)
        {
            // Update the UI with countdown and progressBar

            FurnaceRecipe recipe = furnaceSystem.GetRecipe();
            ItemSlotData outputItem = furnaceSystem.GetOutputItem();

            if (recipe == null) yield break;
            if (outputItem != null)
            {
                if (recipe.OutputItemName != outputItem.itemData.itemName) yield break;
            }
            if (!furnaceSystem.FurnaceHasResources()) yield break; // Stop the coroutine

            float elapsedTime = 0f;
            float totalTime = recipe.CompletionTimeSeconds;

            Vector2 initialSize = new(progressBar.sizeDelta.x, progressBar.sizeDelta.y);
            Vector2 initialPosition = new(progressBar.anchoredPosition.x, progressBar.anchoredPosition.y);

            while (elapsedTime < totalTime)
            {
                if (GetFuelItem() == null || GetInputItem() == null)
                {
                    progressBar.sizeDelta = initialSize;
                    progressBar.anchoredPosition = initialPosition;
                    UpdateVisual();
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                float elapsedMinutes = (totalTime - elapsedTime) / 60f; // Convert to minutes
                float progress = Mathf.Clamp01(elapsedTime / totalTime);

                if (elapsedMinutes < 1f)
                {
                    progressText.text = "> 1 min";
                }
                else
                {
                    progressText.text = Mathf.Ceil(elapsedMinutes).ToString() + " min";
                }

                // Calculate new width
                float newWidth = initialSize.x * (1f + progress);

                // Update sizeDelta and anchoredPosition
                progressBar.sizeDelta = new Vector2(newWidth, initialSize.y);
                progressBar.anchoredPosition = new Vector2(
                    initialPosition.x - (initialSize.x - newWidth) * 0.5f,
                    initialPosition.y
                );

                yield return null; // Wait until the next frame
            }

            progressText.text = "";

            // Ensure it fully shrinks at the end
            progressBar.sizeDelta = new Vector2(0f, initialSize.y);
            progressBar.anchoredPosition = new Vector2(
                initialPosition.x - (initialSize.x * 0.5f),
                initialPosition.y
            );

            yield return new WaitForSeconds(0.01f);

            progressBar.sizeDelta = initialSize;
            progressBar.anchoredPosition = initialPosition;

            furnaceSystem.CreateOutput(); // Consumes and creates output

            UpdateVisual();

            GameEventsManager.Instance.furnaceEvents.FurnaceComplete(furnaceSystem.GetOutputItem());

            yield return null;
        }
        
    }

    private void UpdateVisual()
    {
        progressText.text = "";

        inputSlot.Display(furnaceSystem.GetInputItem());
        fuelSlot.Display(furnaceSystem.GetFuelItem());

        ItemSlotData output = furnaceSystem.GetOutputItem();
        resultSlot.Display(output);
    }
}