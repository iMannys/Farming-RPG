using System;
using UnityEngine;

public class InteractEvents
{
    public event Action<ItemData> onInteract;

    public void StartQuest(ItemData itemData)
    {
        if (onInteract != null)
        {
            onInteract(itemData);
        }
    }
}
