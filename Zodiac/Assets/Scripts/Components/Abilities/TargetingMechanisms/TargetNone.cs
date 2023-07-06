using System.Collections.Generic;
using UnityEngine;

public class TargetNone : ITargetingMechanism
{
    public bool HandleInput(ZodiacInputMap inputMap)
    {
        // we don't do any targeting so just return here
        return true;
    }
}