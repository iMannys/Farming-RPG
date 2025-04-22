using System.Collections.Generic;
using UnityEngine;

public class QuestListingManager : MonoBehaviour
{
    public GameObject listingGrid;
    public GameObject questUIObject;

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;

        Dictionary<string, Quest> questMap = QuestManager.Instance.GetQuestMap();

        foreach (Quest quest in questMap.Values)
        {
            QuestStateChange(quest);
        }
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private bool QuestExistsInList(Quest quest)
    {
        foreach (Transform uiObject in listingGrid.transform)
        {
            QuestListing questListing = uiObject.GetComponent<QuestListing>();
            if (questListing)
            {
                if (questListing.questInfo.id == quest.info.id)
                {
                    return true;
                } 
            }
        }
        return false;
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.state != QuestState.IN_PROGRESS)
        {
            foreach (Transform questUI in listingGrid.transform)
            {
                QuestListing uiListing = questUI.GetComponent<QuestListing>();
                if (uiListing)
                {
                    if (uiListing.questInfo.id == quest.info.id)
                    {
                        Destroy(uiListing.gameObject);
                    }
                }
            }
            return;
        }
        if (QuestExistsInList(quest) != false) return;

        GameObject newQuestUI = Instantiate(questUIObject, listingGrid.transform) as GameObject;
        newQuestUI.SetActive(true);

        QuestListing questListing = newQuestUI.GetComponent<QuestListing>();

        questListing.SetQuestInfo(quest.info);
    }
}
