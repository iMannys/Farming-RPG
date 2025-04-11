using UnityEngine;

public class AdvancementQuestStep : QuestStep
{
    public int levelToReach = 1;
    private bool levelGained;

    private void OnEnable()
    {
        GameEventsManager.Instance.skillEvents.onLevelChanged += HandleLevelChanged;
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
