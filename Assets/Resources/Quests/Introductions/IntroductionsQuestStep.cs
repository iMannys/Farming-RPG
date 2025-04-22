using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IntroductionsQuestStep : QuestStep
{
    private int charactersGreeted;
    public int charactersToGreet = 4;
    public int friendPointsReward = 100;

    private void OnEnable()
    {
        GameEventsManager.Instance.characterEvents.onCharacterUnlock += HandleCharacterUnlock;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.characterEvents.onCharacterUnlock -= HandleCharacterUnlock;
    }

    private void HandleCharacterUnlock(CharacterData characterData)
    {
        if (RelationshipStats.FirstMeeting(characterData))
        {
            if (charactersGreeted < charactersToGreet)
            {
                charactersGreeted++;
                UpdateState();
            }
        }

        if (charactersGreeted >= charactersToGreet)
        {
            List<CharacterData> characters = NPCManager.Instance.Characters();

            foreach (CharacterData NPCData in characters)
            {
                try
                {
                    RelationshipStats.AddFriendPoints(NPCData, friendPointsReward);
                } 
                catch (System.Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
                
            }

            FinishQuestStep();
        }

    }

    private void UpdateState()
    {
        string state = charactersGreeted.ToString();
        ChangeState(state);
    }
}
