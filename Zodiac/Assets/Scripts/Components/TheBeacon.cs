using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class TheBeacon : ZodiacComponent
{
    public override List<IInteraction> GetInteractions()
    {
        return new List<IInteraction> { new FreezeTimeForWielder(), new InteractionSerialize() };
    }
    public override bool HandleEvent(PickedUpEvent e)
    {
        if(e.pickerUpper == GameManager.Instance.ThePlayer)
        {
            MenuManager.Instance.Log($"A NEW HAND TOUCHES THE {GetComponent<Visual>().DisplayName.ToUpper()}.");
        }
        return true;
    }
}