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
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
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