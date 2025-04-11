using SoCollection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Cutscene", menuName ="Cutscene/Cutscene")]
public class Cutscene : ScriptableObject, IConditional
{
    public BlackboardCondition[] conditions;
    //Whether this event cutscene can play again once it has occured.
    public bool recurring;

    public SoCollection<CutsceneAction> action;

    //Check if the condition is met
    public bool CheckConditions(out int score)
    {
        IConditional conditional = this;
        return conditional.CheckConditions(conditions, out score);
    }

}
