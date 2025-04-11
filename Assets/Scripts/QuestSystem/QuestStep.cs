using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    private int stepIndex;

    public void InitializeQuestStep(string questId, int stepIndex)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
    }
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;

            GameEventsManager.Instance.questEvents.AdvanceQuest(questId);

            Destroy(this.gameObject);
        }
    }

    protected void ChangeState(string newState)
    {
        GameEventsManager.Instance.questEvents.QuestStepStateChange(questId, stepIndex, new QuestStepState(newState));
    }
}
