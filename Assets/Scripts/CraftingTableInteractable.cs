using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTableInteractable : InteractableObject
{
    public override void Pickup()
    {
        GameObject craftingSystem = UIManager.Instance.craftingSystemUI;
        craftingSystem.SetActive(true);
        UIManager.Instance.darkBackground.SetActive(true);
    }
}
