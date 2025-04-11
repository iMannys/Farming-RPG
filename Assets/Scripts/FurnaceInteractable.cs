using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceInteractable : InteractableObject
{
    public override void Pickup()
    {
        GameObject furnaceSystem = UIManager.Instance.furnaceUI;
        furnaceSystem.SetActive(true);
        UIManager.Instance.darkBackground.SetActive(true);
    }
}
