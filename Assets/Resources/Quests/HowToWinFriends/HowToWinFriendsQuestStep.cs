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

    private void HandleCharacterGift(CharacterData characterData)
    {
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
