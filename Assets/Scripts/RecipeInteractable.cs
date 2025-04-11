using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeInteractable : InteractableObject
{
    public override void Pickup()
    {
        GameObject recipeBook = UIManager.Instance.recipeBook;
        recipeBook.SetActive(true);
    }
}
