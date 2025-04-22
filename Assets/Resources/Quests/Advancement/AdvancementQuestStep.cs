using UnityEngine;

public class AdvancementQuestStep : QuestStep
{
    public int levelToReach = 1;
    private bool levelGained;

    private void Start()
    {
        GameEventsManager.Instance.skillEvents.onLevelChanged += HandleLevelChanged;

        // Check if already at correct level and we missed the event
        Skill skill = SkillManager.Instance.GetSkill(SkillType.Farming);
        if (skill.level >= levelToReach)
        {
            HandleLevelChanged(skill, skill.level - 1);
        }
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.skillEvents.onLevelChanged -= HandleLevelChanged;
    }

    private void HandleLevelChanged(Skill skill, int previousLevel)
    {
        if (!levelGained)
        {
            if (skill.type == SkillType.Farming && skill.level >= levelToReach)
            {
                levelGained = true;
                UpdateState();
            }
        }

        if (levelGained)
        {
            FinishQuestStep();
        }

    }

    private void UpdateState()
    {
        string state = levelGained.ToString();
        ChangeState(state);
    }
}
