using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;

public class UIManager : MonoBehaviour, ITimeTracker
{
    public static UIManager Instance { get; private set; }

    public Canvas canvas;

    [Header("Screen Management")]
    public GameObject menuScreen;
    public enum Tab
    {
        Inventory, Relationships, Animals, Skills, Quests
    }
    //The current selected tab
    public Tab selectedTab;

    [Header("Status Bar")]
    //Tool equip slot on the status bar
    public Image toolEquipSlot;
    //Tool Quantity text on the status bar
    public TextMeshProUGUI toolQuantityText;
    //Time UI
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;


    [Header("Inventory System")]
    //The inventory panel
    public GameObject inventoryPanel;

    //The tool equip slot UI on the Inventory panel
    public HandInventorySlot toolHandSlot;

    //The tool slot UIs
    public InventorySlot[] toolSlots;

    //The item equip slot UI on the Inventory panel
    public HandInventorySlot itemHandSlot;

    //The item equip slot UI on the Crafting panel
    public HandInventorySlot itemHandSlotCrafting;

    //The item slot UIs
    public InventorySlot[] itemSlots;

    // The crafting item slots
    public InventorySlot[] itemSlotsCrafting;

    // The furnace item slots
    public InventorySlot[] itemSlotsFurnace;

    // The item equip slot UI on the furnace panel
    public HandInventorySlot itemHandSlotFurnace;

    [Header("Item info box")]
    public GameObject itemInfoBox;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Vector2 itemInfoBoxOffset = new Vector2(20f, -20f);

    [Header("Screen Transitions")]
    public GameObject fadeIn;
    public GameObject fadeOut;

    [Header("Dark Background")]
    public GameObject darkBackground;

    [Header("Prompts")]
    public YesNoPrompt yesNoPrompt;
    public NamingPrompt namingPrompt;
    [SerializeField] InteractBubble interactBubble;

    [Header("Player Stats")]
    public TextMeshProUGUI moneyText;

    [Header("Shop")]
    public ShopListingManager shopListingManager;

    [Header("Relationships")]
    public RelationshipListingManager relationshipListingManager;
    public AnimalListingManager animalRelationshipListingManager;

    [Header("Calendar")]
    public GameObject calendarUI;
    public CalendarUIListing calendar;

    [Header("Crafting")]
    public GameObject craftingSystemUI;

    [Header("RecipeBook")]
    public GameObject recipeBook;

    [Header("Furnace")]
    public GameObject furnaceUI;

    [Header("Stamina")]
    public Sprite[] staminaUI;
    public Image StaminaUIImage;
    public int staminaCount;
    public int staminaCountTired;
    public int staminaCountTooTired;

    [Header("Weather")]
    public Sprite[] weatherUI;
    public Image WeatherUIImage;

    [Header("Skills")]
    public SkillListingManager skillListingManager;

    [Header("Quests")]
    public QuestListingManager questListingManager;

    [Header("Pop-up Message")]
    public GameObject popUplistingGrid;
    public GameObject popUpUIObject;
    public float messageDisplayDuration = 2.5f;

    [Header("Icon Sprites")]
    public Sprite[] iconSprites;

    public bool IsDragging {  get; set; }
    public bool IsMainInventory { get; set; } = false;
    public bool IsCraftingInventory { get; set; } = false;
    public bool IsFurnaceInventory { get; set; } = false;
    public int DragIndex { get; set; }
    public InventorySlot Slot { get; set; }
    public InventorySlot.InventoryType? Type { get; set; }
    public ItemSlot ItemSlotParent { get; set; }

    public enum IconType
    {
        Info, Warning, LevelUp, StaminaHigh, StaminaLow
    }

    private Dictionary<IconType, Sprite> iconDictionary;

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

    private void Start()
    {
        PlayerStats.InitializeStats(TimeManager.Instance.GetGameTimestamp());
        PlayerStats.RestoreStamina();
        RenderInventory();
        AssignSlotIndexes();
        RenderPlayerStats();
        DisplayItemInfo(null);
        InitializeCrafting();
        InitializeFurnace();
        InitializeSkills();
        BuildIconDictionary();

        //Add UIManager to the list of objects TimeManager will notify when the time updates
        TimeManager.Instance.RegisterTracker(this);

        staminaCountTired = 45;
        staminaCountTooTired = 0;
    }

    private void Update()
    {
        UpdateItemInfoBoxPosition();
    }

    public bool IsUIActive(string uiName = "")
    {
        string[] nameExclude = new string[] { "FadeIn", "FadeOut", "Menu Button", "PlayerStats", "Stamina", "Skills", "Pop-Up Messages", "PopUpGrid" };

        bool isActive = false;

        if (uiName == string.Empty)
        {
            foreach (Transform uiTransform in transform.Find("Canvas"))
            {
                GameObject uiObject = uiTransform.gameObject;

                bool isExcluded = false;
                foreach (string name in nameExclude)
                {
                    if (name == uiObject.name)
                    {
                        isExcluded = true;
                        break;
                    }
                }


                if (!isExcluded && uiObject.activeSelf)
                {
                    isActive = true;
                    break;
                }
            }
        }
        else
        {
            foreach (Transform uiTransform in transform.Find("Canvas"))
            {
                GameObject uiObject = uiTransform.gameObject;

                bool isExcluded = false;
                foreach (string name in nameExclude)
                {
                    if (name == uiObject.name)
                    {
                        isExcluded = true;
                        break;
                    }
                }


                if (!isExcluded && uiObject.activeSelf && uiObject.name == uiName)
                {
                    isActive = true;
                    break;
                }
            }
        }

        return isActive;
    }

    public void CloseOpenUIElements()
    {
        if (yesNoPrompt.gameObject.activeSelf)
        {
            yesNoPrompt.gameObject.SetActive(false);
            darkBackground.SetActive(false);
        }

        if (shopListingManager.gameObject.activeSelf)
        {
            shopListingManager.gameObject.SetActive(false);
            darkBackground.SetActive(false);
        }

        if (menuScreen.activeSelf)
        {
            menuScreen.gameObject.SetActive(false);
            darkBackground.SetActive(false);
        }

        if (calendarUI.activeSelf)
        {
            calendarUI.gameObject.SetActive(false);
            darkBackground.SetActive(false);
        }
    
        if (craftingSystemUI.activeSelf)
        {
            craftingSystemUI.gameObject.SetActive(false);
            darkBackground.SetActive(false);
        }

        if (recipeBook.activeSelf)
        {
            recipeBook.gameObject.SetActive(false);
            darkBackground.SetActive(false);
        }

        if (furnaceUI.activeSelf)
        {
            furnaceUI.gameObject.SetActive(false);
            darkBackground.SetActive(false);
        }
    }

    #region Skills

    public void InitializeSkills()
    {
        SkillSystem skillSystem = new();

        SkillManager.Instance.SetSkillSystem(skillSystem);
    }

    #endregion Skills

    #region Cooking

    public void InitializeFurnace()
    {
        RenderFurnaceInventory();

        FurnaceSystem furnaceSystem = new();

        UI_FurnaceSystem.Instance.SetFurnaceSystem(furnaceSystem);
    }

    public void RenderFurnaceInventory()
    {
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        //Render the Item section
        RenderInventoryPanel(inventoryItemSlots, itemSlotsFurnace);

        itemHandSlotFurnace.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item));
    }

    #endregion

    #region Crafting

    public void InitializeCrafting()
    {
        RenderCraftingInventory();

        CraftingSystem craftingSystem = new();

        UI_CraftingSystem.Instance.SetCraftingSystem(craftingSystem);
    }

    public void RenderCraftingInventory()
    {
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        //Render the Item section
        RenderInventoryPanel(inventoryItemSlots, itemSlotsCrafting);
        
        itemHandSlotCrafting.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item));
    }

    #endregion
    
    #region Prompts

    public void TriggerNamingPrompt(string message, System.Action<string> onConfirmCallback)
    {
        //Check if another prompt is already in progress
        if (namingPrompt.gameObject.activeSelf)
        {
            //Queue the prompt
            namingPrompt.QueuePromptAction(() => TriggerNamingPrompt(message, onConfirmCallback));
            return;
        }

        //Open the panel
        namingPrompt.gameObject.SetActive(true);

        namingPrompt.CreatePrompt(message, onConfirmCallback);
    }

    public void TriggerYesNoPrompt(string message, System.Action onYesCallback)
    {
        //Set active the gameobject of the Yes No Prompt
        yesNoPrompt.gameObject.SetActive(true);

        yesNoPrompt.CreatePrompt(message, onYesCallback);
    }
    #endregion

    #region Tab Management
    public void ToggleMenuPanel()
    {
        menuScreen.SetActive(!menuScreen.activeSelf);

        OpenWindow(selectedTab);

        darkBackground.SetActive(menuScreen.activeSelf);

        TabBehaviour.onTabStateChange?.Invoke();
    }

    //Manage the opening of windows associated with the tab
    public void OpenWindow(Tab windowToOpen)
    {
        //Disable all windows
        relationshipListingManager.gameObject.SetActive(false);
        inventoryPanel.SetActive(false);
        animalRelationshipListingManager.gameObject.SetActive(false);
        skillListingManager.gameObject.SetActive(false);
        questListingManager.gameObject.SetActive(false);

        //Open the corresponding window and render it
        switch (windowToOpen)
        {
            case Tab.Inventory:
                inventoryPanel.SetActive(true);
                RenderInventory();
                break;
            case Tab.Relationships:
                relationshipListingManager.gameObject.SetActive(true);
                relationshipListingManager.Render(RelationshipStats.relationships);
                break;
            case Tab.Animals:
                animalRelationshipListingManager.gameObject.SetActive(true);
                animalRelationshipListingManager.Render(AnimalStats.animalRelationships);
                break;
            case Tab.Skills:
                skillListingManager.gameObject.SetActive(true);
                break;
            case Tab.Quests:
                questListingManager.gameObject.SetActive(true);
                break;
        }

        //Set the selected tab
        selectedTab = windowToOpen;

        darkBackground.SetActive(inventoryPanel.activeSelf || 
                             relationshipListingManager.gameObject.activeSelf || 
                             animalRelationshipListingManager.gameObject.activeSelf || 
                             skillListingManager.gameObject.activeSelf ||
                             questListingManager.gameObject.activeSelf);
    }
    #endregion

    #region Fadein Fadeout Transitions

    public void FadeOutScreen()
    {
        fadeOut.SetActive(true);
    }

    public void FadeInScreen()
    {
        fadeIn.SetActive(true);
    }

    public void OnFadeInComplete()
    {
        //Disable Fade in Screen when animation is completed
        fadeIn.SetActive(false);
    }

    //Reset the fadein fadeout screens to their default positions
    public void ResetFadeDefaults()
    {
        fadeOut.SetActive(false);
        fadeIn.SetActive(true);
    }



    #endregion

    #region Inventory
    //Iterate through the slot UI elements and assign it its reference slot index
    public void AssignSlotIndexes()
    {
        for (int i =0; i<toolSlots.Length; i++)
        {
            toolSlots[i].AssignIndex(i);
            itemSlots[i].AssignIndex(i);
            itemSlotsCrafting[i].AssignIndex(i);
            itemSlotsFurnace[i].AssignIndex(i);
        }
    }

    //Render the inventory screen to reflect the Player's Inventory.
    public void RenderInventory()
    {
        //Get the respective slots to process
        ItemSlotData[] inventoryToolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        //Render the Tool section
        RenderInventoryPanel(inventoryToolSlots, toolSlots);

        //Render the Item section
        RenderInventoryPanel(inventoryItemSlots, itemSlots);

        //Render the equipped slots
        toolHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool));
        itemHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item));

        //Get Tool Equip from InventoryManager
        ItemData equippedTool = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);

        //Text should be empty by default
        toolQuantityText.text = "";
        //Check if there is an item to display
        if (equippedTool != null)
        {
            //Switch the thumbnail over
            toolEquipSlot.sprite = equippedTool.thumbnail;

            toolEquipSlot.gameObject.SetActive(true);

            //Get quantity
            int quantity = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool).quantity;
            if (quantity > 1)
            {
                toolQuantityText.text = quantity.ToString();
            }
            return;
        }

        toolEquipSlot.gameObject.SetActive(false);
    }

    //Iterate through a slot in a section and display them in the UI
    void RenderInventoryPanel(ItemSlotData[] slots, InventorySlot[] uiSlots)
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            //Display them accordingly
            uiSlots[i].Display(slots[i]);
        }
    }

    public void ToggleInventoryPanel()
    {
        //If the panel is hidden, show it and vice versa
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        RenderInventory();
    }

    //Display Item info on the Item infobox
    public void DisplayItemInfo(ItemData data)
    {
        //If data is null, reset
        if(data == null)
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
            itemInfoBox.SetActive(false);
            return;
        }
        itemInfoBox.SetActive(true);
        itemNameText.text = data.name;
        itemDescriptionText.text = data.description;
    }

    private void UpdateItemInfoBoxPosition()
    {
        if (itemInfoBox.activeSelf)
        {
            Vector2 localPoint;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint
            );

            Vector2 adjustedPos = localPoint + itemInfoBoxOffset;
            itemInfoBox.GetComponent<RectTransform>().anchoredPosition = adjustedPos;
        }
    }

    #endregion

    #region Time
    //Callback to handle the UI for time
    public void ClockUpdate(GameTimestamp timestamp)
    {
        //Handle the time
        //Get the hours and minutes
        int hours = timestamp.hour;
        int minutes = timestamp.minute;
        minutes = (minutes / 10) * 10;

        //AM or PM
        string prefix = "AM ";

        //Convert hours to 12 hour clock
        if(hours >= 12)
        {
            //Time becomes PM
            prefix = "PM ";
            //12 PM and later
            hours -= 12;

        }
        //Special case for 12am/pm to display it as 12 instead of 0
        hours = hours == 0 ? 12 : hours;

        //Format it for the time text display
        timeText.text = prefix + hours + ":" + minutes.ToString("00");

        //Handle the Date
        int day = timestamp.day;
        string season = timestamp.season.ToString();
        string dayOfTheWeek = timestamp.GetDayOfTheWeek().ToString();

        //Format it for the date text display
        dateText.text = season + " " + day + " (" + dayOfTheWeek +")";

    }
    #endregion

    //Render the UI of the player stats in the HUD
    public void RenderPlayerStats()
    {
        moneyText.text = PlayerStats.Money + PlayerStats.CURRENCY;
        staminaCount = PlayerStats.Stamina;
        ChangeStaminaUI();

    }

    //Open the shop window with the shop items listed
    public void OpenShop(List<ItemData> shopItems)
    {
        //Set active the shop window
        shopListingManager.gameObject.SetActive(true);
        shopListingManager.Render(shopItems);
    }

    public void ToggleRelationshipPanel()
    {
        ToggleMenuPanel();
        GameObject panel = relationshipListingManager.gameObject;
        panel.SetActive(!panel.activeSelf);

        //If open, render the screen
        if (panel.activeSelf)
        {
            relationshipListingManager.Render(RelationshipStats.relationships);
        }
    }

    public void InteractPrompt(Transform item, string message, float offsetX, float offsetY, float offsetZ)
    {
        interactBubble.gameObject.SetActive(true);
        interactBubble.transform.position = item.transform.position + new Vector3(offsetX, offsetY, offsetZ);
        interactBubble.Display(message);
    }

    public void DeactivateInteractPrompt()
    {
        interactBubble.gameObject.SetActive(false);
    }

    public void ChangeStaminaUI()
    {
        if (staminaCount <= 45) StaminaUIImage.sprite = staminaUI[3]; // exhausted
        else if (staminaCount <= 80) StaminaUIImage.sprite = staminaUI[2]; // tired
        else if (staminaCount <= 115) StaminaUIImage.sprite = staminaUI[1]; // active
        else if (staminaCount <= 150) StaminaUIImage.sprite = staminaUI[0]; // energised

    }

    public void ChangeWeatherUI()
    {
        var WeatherToday = WeatherManager.Instance.WeatherToday;

        switch (WeatherToday)
        {
            case WeatherData.WeatherType.Sunny:
                WeatherUIImage.sprite = weatherUI[0];
                break;
            case WeatherData.WeatherType.Rain:
                WeatherUIImage.sprite = weatherUI[1];
                break;
            case WeatherData.WeatherType.Snow:
                WeatherUIImage.sprite = weatherUI[2];
                break;
            case WeatherData.WeatherType.HeavySnow:
                WeatherUIImage.sprite = weatherUI[3];
                break;
            case WeatherData.WeatherType.Typhoon:
                WeatherUIImage.sprite = weatherUI[4];
                break;
        }
    }

    private void BuildIconDictionary()
    {
        iconDictionary = new Dictionary<IconType, Sprite>();

        // Make sure the order of sprites in iconSprites matches the enum order
        for (int i = 0; i < iconSprites.Length && i < System.Enum.GetValues(typeof(IconType)).Length; i++)
        {
            iconDictionary[(IconType)i] = iconSprites[i];
        }
    }

    private void AddPopupMessageToQueue(Sprite icon, string message)
    {
        GameObject newPopupListing = Instantiate(popUpUIObject, popUplistingGrid.transform);
        newPopupListing.SetActive(true);

        PopUpListing popUpListing = newPopupListing.GetComponent<PopUpListing>();
        popUpListing.SetPopupMessage(icon, message);
        popUpListing.StartFadeAndDestroy(messageDisplayDuration, 0.5f);
    }

    public void ShowPopupMessage(IconType iconType, string message)
    {
        if (iconDictionary.TryGetValue(iconType, out Sprite icon))
        {
            AddPopupMessageToQueue(icon, message);
        }
        else
        {
            Debug.LogWarning("IconType not found in dictionary: " + iconType);
        }
    }
}
