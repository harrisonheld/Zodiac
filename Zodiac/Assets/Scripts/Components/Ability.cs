using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ZodiacComponent
{
    public int RechargeTime { get; set; } = 10;
    public int Cooldown { get; set; } = 0;

    public virtual void Activate()
    {
        Cooldown = RechargeTime;
    }
}