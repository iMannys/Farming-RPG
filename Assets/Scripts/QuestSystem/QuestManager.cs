using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, Quest> questMap;

    private List<QuestLevel> currentLevels;

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

        questMap = CreateQuestMap();
        currentLevels = new List<QuestLevel>();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.Instance.skillEvents.onLevelChanged += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.Instance.skillEvents.onLevelChanged -= PlayerLevelChange;
    }

    private void Start()
    {
        Skill[] skills = SkillManager.Instance.GetAllSkills();

        foreach (Skill skill in skills)
        {
            currentLevels.Add(new QuestLevel(skill.type, skill.level));
        }

        foreach (Quest quest in questMap.Values)
        {
            if (quest.info.useQuestPoints == false && quest.state <= QuestState.CAN_START) // CAN_START and REQUIREMENTS_NOT_MET
            {
                if (CheckRequirementsMet(quest))
                {
                    ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
                }
            }

            // Initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }

            // Broadcast initial state of all quests
            GameEventsManager.Instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(Skill skill, int previousLevel)
    {
        foreach(QuestLevel questLevel in currentLevels)
        {
            if (skill.type == questLevel.type)
            {
                questLevel.level = skill.level;
            }
        }
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirement = true;

        QuestLevel currentQuestLevel = null;

        foreach(QuestLevel currentLevel in currentLevels)
        {
            QuestLevel questLevel = quest.info.levelRequirement;
            if (questLevel.type == currentLevel.type)
            {
                currentQuestLevel = currentLevel;
            }
        }
        // If the currentQuestLevel is null we know the type is None
        if (currentQuestLevel != null)
        {
            if (currentQuestLevel.level < quest.info.levelRequirement.level)
            {
                meetsRequirement = false;
            }
        }

        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirement = false;
            }
        }

        return meetsRequirement;
    }

    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            if ((quest.state == QuestState.REQUIREMENTS_NOT_MET || quest.state == QuestState.CAN_START) && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
            }
        }
    }

    public Dictionary<string, Quest> GetQuestMap() 
    {
        return questMap;
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            if (quest.info.useQuestPoints)
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
            }
            else
            {
                GameEventsManager.Instance.questEvents.FinishQuest(quest.info.id);
            }
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        foreach (Quest otherQuest in questMap.Values)
        {
            if ((otherQuest.state == QuestState.REQUIREMENTS_NOT_MET || otherQuest.state == QuestState.CAN_START)
                && CheckRequirementsMet(otherQuest))
            {
                ChangeQuestState(otherQuest.info.id, QuestState.IN_PROGRESS);
                otherQuest.InstantiateCurrentQuestStep(this.transform);
            }
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.questComplete);
        UIManager.Instance.ShowPopupMessage(UIManager.IconType.LevelUp, $"You completed the quest {quest.info.displayName}!");
    }

    private void ClaimRewards(Quest quest)
    {
        if (quest.info.goldReward > 0)
        {
            PlayerStats.Earn(quest.info.goldReward);
        }

        if (quest.info.experienceReward > 0 && quest.info.levelRequirement.type != SkillType.None)
        {
            Skill skill = SkillManager.Instance.GetSkill(quest.info.levelRequirement.type);

            SkillManager.Instance.AddExperience(skill, quest.info.experienceReward);
        }
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // Load all quests under Assets/Resources/Quests
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate Id found when creating quest map: " +  questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }

    /*
    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            // Serialize data with JSONUtility
            string serializedData = JsonUtility.ToJson(questData);

            //PlayerPrefs.SetString(quest.info.id, serializedData);
        } 
        catch (System.Exception e)
        {
            Debug.Log("Failed to save quest with id: " + quest.info.id + ": " + e);
        }
    }
    */
}
