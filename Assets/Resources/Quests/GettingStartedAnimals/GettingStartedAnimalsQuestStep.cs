using UnityEngine;

public class GettingStartedAnimalsQuestStep : QuestStep
{
    private bool eggHarvested;

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
        if (!eggHarvested)
        {
            if (itemData.itemName == "Egg")
            {
                eggHarvested = true;
                UpdateState();
            }
        }

        if (eggHarvested)
        {
            FinishQuestStep();
        }

    }

    private void UpdateState()
    {
        string state = eggHarvested.ToString();
        ChangeState(state);
    }
}
