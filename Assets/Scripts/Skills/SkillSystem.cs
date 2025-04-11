using System;
using UnityEngine;

public enum SkillType
{
    Farming,
    Crafting,
    Cooking,
    Barter,
    AnimalCaring,
    None,
}

public class Skill
{
    public SkillType type;

    public SkillSystem skillSystem;
    public int level;
    public int experience;
    public int experienceAtCurrentLevel;
    public int experienceToNextLevel;

    public Skill(SkillSystem skillSystem, SkillType type, int level, int experience)
    {
        this.skillSystem = skillSystem;
        this.type = type;
        this.level = level;
        this.experience = experience;
        this.experienceAtCurrentLevel = this.skillSystem.GetXPAtCurrentLevel(this);
        this.experienceToNextLevel = this.skillSystem.GetXPToNextLevel(this);
    }
}

public class SkillSystem
{
    public Skill[] skills;
    private readonly int[] experienceForLevels;

    public SkillSystem()
    {
        experienceForLevels = new int[]
        {
            100,
            380,
            770,
            1300,
            2150,
            3300,
            4800,
            6900,
            10000,
            15000
        };

        skills = new Skill[]
        {
            new(this, SkillType.Farming, 0, 0),
            new(this, SkillType.Crafting, 0, 0),
            new(this, SkillType.Cooking, 0, 0),
            new(this, SkillType.Barter, 0, 0),
            new(this, SkillType.AnimalCaring, 0, 0)
        };
    }

    public bool IsMaxLevel(Skill skill)
    {
        if (skill.level >= experienceForLevels.Length)
        {
            return true;
        }
        return false;
    }
    
    public void UpdateLevel(Skill skill)
    {
        if (IsMaxLevel(skill)) return;

        for (int i = 0; i < experienceForLevels.Length; i++)
        {
            int currentXP = experienceForLevels[i];
            int nextXP = currentXP == experienceForLevels[^1] ? experienceForLevels[^1] : experienceForLevels[i + 1];

            if (skill.experience >= currentXP && skill.experience < nextXP)
            {
                skill.level = i + 1;
                skill.experienceAtCurrentLevel = currentXP;
                skill.experienceToNextLevel = nextXP;
                GameEventsManager.Instance.skillEvents.LevelChanged(skill, i);
                return;
            } else continue;
        }

        // If the experience is higher than what the level is then return the highest level 
        skill.level = experienceForLevels.Length;
        skill.experienceAtCurrentLevel = experienceForLevels[^1];
        skill.experienceToNextLevel = experienceForLevels[^1];
        GameEventsManager.Instance.skillEvents.LevelChanged(skill, skill.level);
    }

    public int GetXPAtCurrentLevel(Skill skill)
    {
        if (skill.level <= 0)
        {
            return 0;
        }
        return experienceForLevels[skill.level - 1];
    }

    public int GetXPToNextLevel(Skill skill)
    {
        if (skill.level > experienceForLevels.Length)
        {
            return experienceForLevels[^1]; // ^1 represents the last element in the array ^2 would be the second last element
        } else if (skill.level < 0)
        {
            return experienceForLevels[0];
        }
        return experienceForLevels[skill.level];
    }

    public void AddExperience(Skill skill, int amount)
    {
        skill.experience += amount;
        if (skill.experience >= skill.experienceToNextLevel)
        {
            UpdateLevel(skill);
        }
        GameEventsManager.Instance.skillEvents.ExperienceChanged(skill, amount);
    }
}
