using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBeacon : ZodiacComponent
{
    public override bool HandleEvent(PickedUpEvent e)
    {
        if(e.pickerUpper == GameManager.Instance.ThePlayer)
        {
            AlertMenu.Instance.ShowText("A NEW HAND TOUCHES THE BEACON.");
        }
        return true;
    }
}