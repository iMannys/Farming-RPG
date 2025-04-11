using System;
using UnityEngine;

public class FurnaceEvents
{
    public event Action<ItemSlotData> onFurnaceComplete;

    public void FurnaceComplete(ItemSlotData item)
    {
        if (onFurnaceComplete != null)
        {
            onFurnaceComplete(item);
        }
    }
}
