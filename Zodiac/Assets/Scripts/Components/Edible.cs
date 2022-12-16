using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Edible : ZodiacComponent
{
    public override List<IInteraction> GetInteractions()
    {
        return new List<IInteraction> { new SerializePlayerInteraction() };
    }
}