using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public QuestEvents questEvents;
    public SkillEvents skillEvents;
    public FurnaceEvents furnaceEvents;
    public CharacterEvents characterEvents;
    public InteractEvents interactEvents;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        Instance = this;

        // initialize all events
        questEvents = new QuestEvents();
        skillEvents = new SkillEvents();
        furnaceEvents = new FurnaceEvents();
        characterEvents = new CharacterEvents();
        interactEvents = new InteractEvents();
    }
}