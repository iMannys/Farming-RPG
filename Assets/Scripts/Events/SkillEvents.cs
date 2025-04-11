using System;
using UnityEngine;

public class SkillEvents
{
    public event Action<Skill, int> onExperienceChanged;

    public void ExperienceChanged(Skill skill, int experienceGained)
    {
        if (onExperienceChanged != null)
        {
            onExperienceChanged(skill, experienceGained);
        }
    }

    public event Action<Skill, int> onLevelChanged;

    public void LevelChanged(Skill skill, int previousLevel)
    {
        if (onLevelChanged != null)
        {
            onLevelChanged(skill, previousLevel);
        }
    }
}
