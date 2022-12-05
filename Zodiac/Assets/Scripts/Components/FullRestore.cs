using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullRestore : Ability
{
    public override void Activate()
    {
        Health h = GetComponent<Health>();
        h.HealthCurrent = h.HealthMax;

        base.Activate();
    }
}