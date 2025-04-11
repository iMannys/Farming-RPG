using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private SkillSystem skillSystem;
    private SkillPanel[] skillPanelArray;

    AudioManager audioManager;

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

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        Transform skillUI = transform.Find("Canvas").Find("Skills");
        Transform skillGrid = skillUI.GetChild(0);

        skillPanelArray = new SkillPanel[skillGrid.childCount];


        int index = 0;
        foreach (Transform panelTransform in skillGrid)
        {
            if (panelTransform.TryGetComponent<SkillPanel>(out var skillPanel))
            {
                skillPanelArray[index] = skillPanel;
                index++;
            }
        }
    }

    public void SetSkillSystem(SkillSystem system)
    {
        skillSystem = system;

        GameEventsManager.Instance.skillEvents.onExperienceChanged += SkillSystem_OnExperienceChanged;
        GameEventsManager.Instance.skillEvents.onLevelChanged += SkillSystem_OnLevelChanged;
        
        foreach (Skill skill in skillSystem.skills)
        {
            foreach (SkillPanel skillPanel in skillPanelArray)
            {
                if (skillPanel.type == skill.type)
                {
                    skillPanel.SetLevelText(skill.level.ToString());
                    skillPanel.ResetProgress();
                }
            }
        }
    }

    public Skill[] GetAllSkills()
    {
        return skillSystem.skills;
    }

    public Skill GetSkill(SkillType type)
    {
        foreach (Skill skill in skillSystem.skills)
        {
            if (skill.type == type)
            {
                return skill;
            }
        }
        return null;
    }

    public bool IsMaxLevel(Skill skill)
    {
        return skillSystem.IsMaxLevel(skill);
    }

    public void AddExperience(Skill skill, int amount)
    {
        skillSystem.AddExperience(skill, amount);
    }

    private void SkillSystem_OnExperienceChanged(Skill skill, int experienceGained)
    {
        foreach (SkillPanel skillPanel in skillPanelArray)
        {
            if (skillPanel.type == skill.type)
            {
                skillPanel.AnimateProgress(skill, experienceGained);
                break;
            }
        }
    }

    private void SkillSystem_OnLevelChanged(Skill skill, int previousLevel)
    {
        foreach (SkillPanel skillPanel in skillPanelArray)
        {
            if (skillPanel.type == skill.type)
            {
                skillPanel.SetLevelText(skill.level.ToString());
                audioManager.PlaySFX(audioManager.newRecord);
                UIManager.Instance.ShowPopupMessage(UIManager.IconType.LevelUp, $"You just leveled up in {skill.type}!");
                break;
            }
        }
    }
}
