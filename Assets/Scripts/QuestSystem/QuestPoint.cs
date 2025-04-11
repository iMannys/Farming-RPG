using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool isStartPoint = true;
    [SerializeField] private bool isEndPoint = true;
    [SerializeField] private bool activateOnCollide = false;
    [SerializeField] private bool activateOnEnable = false;
    [SerializeField] private bool activateOnCanStart = false;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private void Awake()
    {
        questId = questInfoForPoint.id;
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;

        if (activateOnEnable)
        {
            QuestInit();
        }
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;

            if (currentQuestState == QuestState.CAN_START)
            {
                QuestInit();
            }

            Debug.Log("Quest with id: " + questId + " updated to state: " + currentQuestState);
        }
    }

    private void QuestInit()
    {
        if (!playerIsNear) return;

        if (currentQuestState == QuestState.CAN_START && isStartPoint)
        {
            GameEventsManager.Instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState == QuestState.CAN_FINISH && isEndPoint)
        {
            GameEventsManager.Instance.questEvents.FinishQuest(questId);
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
            
            if (activateOnCollide)
            {
                QuestInit();
            }
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    // Ensures that activateOnCollide, activateOnEnable and activateOnCanStart cannot be true at the same time
    private void OnValidate()
    {
        if (activateOnCollide)
        {
            activateOnEnable = false;
            activateOnCanStart = false;
        }
        else if (activateOnEnable)
        {
            activateOnCollide = false;
            activateOnCanStart = false;
        }
        else if (activateOnCanStart)
        {
            activateOnCollide = false;
            activateOnEnable = false;
        }
    }
}
