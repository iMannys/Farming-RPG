using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObstacleGenerator))]
public class LandManager : MonoBehaviour
{
    public static LandManager Instance { get; private set; }

    public static Tuple<List<LandSaveState>, List<CropSaveState>> farmData = null;

    List<Land> landPlots = new List<Land>();

    //The save states of our land and crops
    List<LandSaveState> landData = new List<LandSaveState>();
    List<CropSaveState> cropData = new List<CropSaveState>();

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }


    void OnEnable()
    {
        RegisterLandPlots();
        StartCoroutine(LoadFarmData());
        
    }

    IEnumerator LoadFarmData()
    {
        yield return new WaitForEndOfFrame();
        //Load farm data if any
        if(farmData != null)
        {
            //Load in any saved data
            ImportLandData(farmData.Item1);
            ImportCropData(farmData.Item2);
        } else
        {
            //New Game
            //Generate Obstacles
            GetComponent<ObstacleGenerator>().GenerateObstacles(landPlots);
        }
    }

    private void OnDestroy()
    {
        //Save the Instance variables over to the static variable
        farmData = new Tuple<List<LandSaveState>, List<CropSaveState>>(landData, cropData);
        cropData.ForEach((CropSaveState crop) => {
            Debug.Log(crop.seedToGrow);
        });
    }

    #region Registering and Deregistering
    //Get all the Land Objects in the scene and cache it
    void RegisterLandPlots()
    {
        foreach(Transform landTransform in transform)
        {
            Land land = landTransform.GetComponent<Land>();
            landPlots.Add(land);

            //Create a corresponding LandSaveState
            landData.Add(new LandSaveState());

            //Assign it an id based on its index
            land.id = landPlots.Count - 1;
        }
    }

    //Registers the crop onto the Instance
    public void RegisterCrop(int landID, SeedData seedToGrow, CropBehaviour.CropState cropState, int growth, int health)
    {
        cropData.Add(new CropSaveState(landID, seedToGrow.name, cropState, growth, health));
    }

    public void DeregisterCrop(int landID)
    {
        //Find its index in the list from the landID and remove it
        cropData.RemoveAll(x => x.landID == landID);
    }
    #endregion

    #region State Changes
    //Update the corresponding Land Data on ever change to the Land's state
    public void OnLandStateChange(int id, Land.LandStatus landStatus, GameTimestamp lastWatered, Land.FarmObstacleStatus obstacleStatus)
    {
        landData[id] = new LandSaveState(landStatus, lastWatered, obstacleStatus);
    }

    //Update the corresponding Crop Data on ever change to the Land's state
    public void OnCropStateChange(int landID, CropBehaviour.CropState cropState, int growth, int health)
    {
        //Find its index in the list from the landID
        int cropIndex = cropData.FindIndex(x => x.landID == landID);

        string seedToGrow = cropData[cropIndex].seedToGrow;
        cropData[cropIndex] = new CropSaveState(landID, seedToGrow, cropState, growth, health);
    }
    #endregion

    #region Loading Data
    //Load over the static farmData onto the Instance's landData
    public void ImportLandData(List<LandSaveState> landDatasetToLoad)
    {
        for(int i =0; i < landDatasetToLoad.Count; i++)
        {
            //Get the individual land save state
            LandSaveState landDataToLoad = landDatasetToLoad[i];
            //Load it up onto the Land instance
            landPlots[i].LoadLandData(landDataToLoad.landStatus, landDataToLoad.lastWatered, landDataToLoad.obstacleStatus);

        }

        landData = landDatasetToLoad;
    }

    //Load over the static farmData onto the Instance's cropData
    public void ImportCropData(List<CropSaveState> cropDatasetToLoad)
    {
        cropData = cropDatasetToLoad;
        foreach (CropSaveState cropSave in cropDatasetToLoad)
        {
            //Access the land
            Land landToPlant = landPlots[cropSave.landID];
            //Spawn the crop
            CropBehaviour cropToPlant = landToPlant.SpawnCrop();
            //Load in the data
            SeedData seedToGrow = (SeedData) InventoryManager.Instance.GetItemFromString(cropSave.seedToGrow);
            cropToPlant.LoadCrop(cropSave.landID, seedToGrow, cropSave.cropState, cropSave.growth, cropSave.health);
        }

    }
    #endregion

    // Update is called once per frame
    void Update()
    {


    }
}
