using UnityEngine;

public class GettingStartedFarmingQuestStep : QuestStep
{
    private bool cabbageHarvested = false;

    private void OnEnable()
    {
        GameEventsManager.Instance.interactEvents.onInteract += HandleInteract;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.interactEvents.onInteract -= HandleInteract;
    }

    private void HandleInteract(ItemData itemData)
    {
        Debug.Log(itemData.itemName);
        if (!cabbageHarvested)
        {
            if (itemData.itemName == "Cabbage")
            {
                cabbageHarvested = true;
                UpdateState();
            }
        }

        if (cabbageHarvested)
        {
            FinishQuestStep();
        }

    }

    private void UpdateState()
    {
        string state = cabbageHarvested.ToString();
        ChangeState(state);
    }
}
