using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public int RechargeTime = 10;
    public int Cooldown = 0;

    public virtual void Activate()
    {
        Cooldown = RechargeTime;
    }
}