using TMPro;
using UnityEngine;

public class QuestListing : MonoBehaviour
{
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questRequirements;
    public TextMeshProUGUI questReward;
    public QuestInfoSO questInfo { get; set; }
    
    public void SetQuestInfo(QuestInfoSO info)
    {
        questInfo = info;
        questName.text = questInfo.displayName;
        questRequirements.text = "Requirements: " + questInfo.requirements;

        HoverDisplay hoverDisplay = transform.GetComponent<HoverDisplay>();

        hoverDisplay.descriptionText = questInfo.description;

        if (questInfo.goldReward > 0)
        {
            questReward.text = questInfo.goldReward.ToString() + "G";
        }
        else if (questInfo.experienceReward > 0)
        {
            questReward.text = questInfo.experienceReward.ToString() + " XP";
        }
        else
        {
            questReward.text = "";
        }
    }
}
