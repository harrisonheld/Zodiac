using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBeacon : ZodiacComponent
{
    public override List<IInteraction> GetInteractions()
    {
        return new List<IInteraction> { new FreezeTimeForWielder(), new ShootYourself(), new SerializePlayerInteraction() };
    }
    public override bool HandleEvent(PickedUpEvent e)
    {
        if(e.pickerUpper == GameManager.Instance.ThePlayer)
        {
            AlertMenu.Instance.ShowText("A NEW HAND TOUCHES THE BEACON.");
        }
        return true;
    }
}