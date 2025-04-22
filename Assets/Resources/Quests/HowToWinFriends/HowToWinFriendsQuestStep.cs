using System.Collections.Generic;
using UnityEngine;

public class HowToWinFriendsQuestStep : QuestStep
{
    private bool hasGiftedCharacter;

    private void OnEnable()
    {
        GameEventsManager.Instance.characterEvents.onCharacterGift += HandleCharacterGift;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.characterEvents.onCharacterGift -= HandleCharacterGift;
    }

    private void Start()
    {
        if (hasGiftedCharacter)
        {
            FinishQuestStep();
        }
    }

    private void HandleCharacterGift(CharacterData characterData)
    {
        Debug.Log($"Gifted a character");

        if (!hasGiftedCharacter)
        {
            hasGiftedCharacter = true;
            UpdateState();
        }

        if (hasGiftedCharacter)
        {
            FinishQuestStep();
        }

    }

    private void UpdateState()
    {
        string state = hasGiftedCharacter.ToString();
        ChangeState(state);
    }
}
