using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneTransitionManager;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }

    public List<StartPoint> startPoints;

    //The connections between scenes
    private static readonly Dictionary<Location, List<Location>> sceneConnections = new Dictionary<Location, List<Location>>() {
        {Location.PlayerHome, new List<Location>{ Location.Farm} },
        {Location.Farm, new List<Location>{ Location.PlayerHome, Location.Town } },
        {Location.Town, new List<Location>{Location.YodelRanch, Location.Forest, Location.Farm, Location.Inn, Location.TownSquare } },
        {Location.TownSquare, new List<Location>{Location.Town} },
        {Location.Inn, new List<Location>{Location.Town} },
        {Location.YodelRanch, new List<Location>{ Location.Town} },
        {Location.Forest, new List<Location>{Location.Town} }
    };

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

    //Find the player's start position based on where he's coming from
    public Transform GetPlayerStartingPosition(SceneTransitionManager.Location enteringFrom)
    {
        //Tries to find the matching startpoint based on the Location given
        StartPoint startingPoint = startPoints.Find(x => x.enteringFrom == enteringFrom);

        //Return the transform
        return startingPoint.playerStart;
    }

    public Transform GetExitPosition(Location exitingTo)
    {
        Transform startPoint = GetPlayerStartingPosition(exitingTo);
        return startPoint.parent.GetComponentInChildren<LocationEntryPoint>().transform;

    }

    //Get the next scene in the traversal path
    public static Location GetNextLocation(Location currentScene, Location finalDestination)
    {
        //Track visited locations
        Dictionary<Location, bool> visited = new Dictionary<Location, bool>();

        //Store previous location in the traversal path
        Dictionary<Location, Location> previousLocation = new Dictionary<Location, Location>();

        //Queue for BFS traversal
        Queue<Location> worklist = new Queue<Location>();

        //Mark the current scene as visited and start the traversal
        visited.Add(currentScene, false);
        worklist.Enqueue(currentScene);

        //BFS traversal
        while (worklist.Count != 0)
        {
            Location scene = worklist.Dequeue();
            if (scene == finalDestination)
            {
                //Reconstruct the path and return the preceding scene
                while (previousLocation.ContainsKey(scene) && previousLocation[scene] != currentScene)
                {
                    scene = previousLocation[scene];
                }
                return scene;

            }
            //Enqueue possible destinations connected to the current scene
            if (sceneConnections.ContainsKey(scene))
            {
                List<Location> possibleDestinations = sceneConnections[scene];

                foreach (Location neighbour in possibleDestinations)
                {
                    if (!visited.ContainsKey(neighbour))
                    {
                        visited.Add(neighbour, false);
                        previousLocation.Add(neighbour, scene);
                        worklist.Enqueue(neighbour);
                    }
                }
            }
        }

        return currentScene;
    }


}