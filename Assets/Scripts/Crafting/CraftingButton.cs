using UnityEngine;

public class CraftingButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ClearTable()
    {
        UI_CraftingSystem.Instance.ClearTableToInventory();
    }
}
