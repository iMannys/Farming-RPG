[System.Serializable]
public class QuestLevel
{
    public SkillType type;
    public int level;

    public QuestLevel(SkillType type, int level)
    {
        this.type = type;
        this.level = level;
    }
}
