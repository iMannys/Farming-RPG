using System;
using UnityEngine;

public class CharacterEvents
{
    public event Action<CharacterData> onCharacterInteract;

    public void CharacterInteract(CharacterData characterData)
    {
        if (onCharacterInteract != null)
        {
            onCharacterInteract(characterData);
        }
    }

    public event Action<CharacterData> onCharacterUnlock;

    public void CharacterUnlock(CharacterData characterData)
    {
        if (onCharacterUnlock != null)
        {
            onCharacterUnlock(characterData);
        }
    }

    public event Action<CharacterData> onCharacterGift;

    public void CharacterGift(CharacterData characterData)
    {
        Debug.Log("CharacterGift event fired!");
        if (onCharacterGift != null)
        {
            onCharacterGift(characterData);
        }
    }
}
