using System.Collections.Generic;
using UnityEngine;

public class TargetNone : ITargetingMechanism
{
    public bool HandleInput()
    {
        // we don't do any targetting so just return here
        return true;
    }
}