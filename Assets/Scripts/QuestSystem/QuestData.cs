using UnityEngine;

[System.Serializable]
public class QuestData
{
    public QuestState state;
    public int questStepIndex;
    public QuestStepState[] questStepStates;

    public QuestData(QuestState questState, int questStepIndex, QuestStepState[] questStepStates)
    {
        this.state = questState;
        this.questStepIndex = questStepIndex;
        this.questStepStates = questStepStates;
    }
}
