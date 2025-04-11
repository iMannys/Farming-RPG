using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public SkillType type;
    public Image image;
    public TextMeshProUGUI levelText;
    public SlicedFilledImage progressBar;

    public float maxTime = 2.5f;
    public float minTime = 1f;

    public void AnimateProgress(Skill skill, int experienceGained)
    {
        if (SkillManager.Instance.IsMaxLevel(skill) && progressBar.fillAmount == 1) return;

        float skillProgress = (float)(skill.experience - skill.experienceAtCurrentLevel) / (skill.experienceToNextLevel - skill.experienceAtCurrentLevel);

        float totalTime = Mathf.Lerp(maxTime, minTime, experienceGained / (float)skill.experienceToNextLevel);

        StartCoroutine(AnimateProgressBar(skillProgress, totalTime));
    }

    private IEnumerator AnimateProgressBar(float targetProgress, float duration)
    {
        float elapsedTime = 0f;
        float initialProgress = progressBar.fillAmount;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            progressBar.fillAmount = Mathf.Lerp(initialProgress, targetProgress, elapsedTime / duration);

            yield return null;
        }

        // Ensure progress reaches its final value
        progressBar.fillAmount = targetProgress;
    }

    public void ResetProgress()
    {
        progressBar.fillAmount = 0;
    }
    
    public void SetLevelText(string text)
    {
        levelText.text = text;
    }
}
