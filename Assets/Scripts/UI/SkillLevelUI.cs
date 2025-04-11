using UnityEngine;
using UnityEngine.UI;

public class SkillLevelUI : MonoBehaviour
{
    public int requiredLevel;
    public SkillType skillType;
    public Image iconImage;
    public Sprite litUpSprite;
    public Sprite dimmedSprite;
    
    private void OnEnable()
    {
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        Skill skill = SkillManager.Instance.GetSkill(skillType);
        if (skill == null) return;

        iconImage.sprite = skill.level >= requiredLevel ? litUpSprite : dimmedSprite;
        iconImage.color = Color.white;
    }
}
