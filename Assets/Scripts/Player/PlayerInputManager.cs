using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public GameObject dialogue;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && dialogue.activeInHierarchy)
        {
            DialogueManager.Instance.UpdateDialogue();
        }
    }
}
