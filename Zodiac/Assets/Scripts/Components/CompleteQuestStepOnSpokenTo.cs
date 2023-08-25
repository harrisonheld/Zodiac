using QuestNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteQuestStepOnSpokenTo : ZodiacComponent
{
    public string QuestId { get; set; }
    public string QuestStepId { get; set; }

    public override bool HandleEvent(SpokenToEvent e)
    {
        QuestManager.Instance.CompleteQuestStep(QuestId, QuestStepId);

        return true;
    }
}