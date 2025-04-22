using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalStats : MonoBehaviour
{

    public const string ANIMAL_COUNT = "AnimalCount";
    public const string ANIMAL_DATA = "AnimalRelationship";

    //The relationship data of all the NPCs that the player has met in the game
    public static List<AnimalRelationshipState> animalRelationships = new List<AnimalRelationshipState>();

    //Load all the animal data scriptable objects
    static List<AnimalData> animals = Resources.LoadAll<AnimalData> ("Animals").ToList();

    //To be fired up when a new anmal is born or purchased
    public static void StartAnimalCreation(AnimalData animalType)
    {
        //Retrieve Blackboard
        GameBlackboard blackboard = GameStateManager.Instance.GetBlackboard();
        //Initialise animal count parameter
        if (!blackboard.TryGetValue(ANIMAL_COUNT, out int animalCount))
        {
            blackboard.SetValue(ANIMAL_COUNT, 0);
            animalCount = 0;
        }
        //Handle stats on animal type
        if (!blackboard.TryGetValue(ANIMAL_COUNT +animalType.name, out int animalTypeCount))
        {
            blackboard.SetValue(ANIMAL_COUNT + animalType.name, 0);
            animalTypeCount = 0;
        }

        //Handle Animal spawning here
        UIManager.Instance.TriggerNamingPrompt($"Give your new {animalType.name} a name.", (inputString) => {
            //Create a new animal and add it to the animal relationships data
            AnimalRelationshipState animalRelationshipData = new AnimalRelationshipState(animalCount, inputString, animalType);
            //Animal entries are set by name
            blackboard.SetValue(ANIMAL_DATA + animalCount, animalRelationshipData);

            //Statistics update
            animalCount++;
            animalTypeCount++;
            blackboard.SetValue(ANIMAL_COUNT, animalCount);
            blackboard.SetValue(ANIMAL_COUNT + animalType.name, animalTypeCount);

            //Add it to our local cache
            animalRelationships.Add(animalRelationshipData);
        });
    }

    //Load in the animal relationships
    public static void LoadStats()
    {
        //Load from the Blackboard data
        animalRelationships = new List<AnimalRelationshipState>();
        GameBlackboard blackboard = GameStateManager.Instance.GetBlackboard();

        //Get the animal count
        if(blackboard.TryGetValue(ANIMAL_COUNT, out int animalCount)){
            for (int i = 0; i < animalCount; i++)
            {
                if (!blackboard.TryGetValue(ANIMAL_DATA + i, out AnimalRelationshipState animalRelationship)) continue;

                animalRelationships.Add(animalRelationship);

            }
        }
    }

    //Get the animals by type
    public static List<AnimalRelationshipState> GetAnimalsByType(string animalTypeName)
    {
        return animalRelationships.FindAll(x => x.animalType == animalTypeName);
    }

    public static List<AnimalRelationshipState> GetAnimalsByType(AnimalData animalType)
    {
        return GetAnimalsByType(animalType.name);
    }

    public static void OnDayReset()
    {
        Skill skill = SkillManager.Instance.GetSkill(SkillType.AnimalCaring);
        GameBlackboard blackboard = GameStateManager.Instance.GetBlackboard();
        //Reset animal relationship states
        foreach(AnimalRelationshipState animal in AnimalStats.animalRelationships)
        {
            //Increase friendship if player has spoken with the anmial
            if (animal.hasTalkedToday)
            {
                int amount = 30;
                animal.friendshipPoints += amount;
                SkillManager.Instance.AddExperience(skill, amount);
            } else
            {
                int amount = -(10 - (animal.friendshipPoints / 200));
                animal.friendshipPoints += amount;
            }

            //Feeding
            if (animal.giftGivenToday)
            {
                animal.Mood += 15;
            }
            else
            {
                int amount = -20;
                animal.Mood -= 100;
                animal.friendshipPoints += amount;
            }

            animal.hasTalkedToday = false;
            //Gift given refers to whether the animal has been fed
            animal.giftGivenToday = false;
            animal.givenProduceToday = false;

            //Advance the age of the animal
            animal.age++;

            //Update its value in the blackboard
            blackboard.SetValue(ANIMAL_DATA + animal.name, animal);

        }
    }

    //Get the animal data type from a string
    public static AnimalData GetAnimalTypeFromString(string name)
    {
        return animals.Find(i => i.name == name);
    }

}
