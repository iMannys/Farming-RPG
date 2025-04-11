using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCondition: IConditional
{
    //An identifier for the dialogue condition for easier editing
    public string id; 
    public BlackboardCondition[] conditions;
    public List<DialogueLine> dialogueLine;


    public bool CheckConditions(out int conditionsMet)
    {
        IConditional conditionChecker = this;
        return conditionChecker.CheckConditions(conditions, out conditionsMet);
    }
}
