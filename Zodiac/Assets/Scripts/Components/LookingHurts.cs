using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LookingHurts : ZodiacComponent
{
    [field: SerializeField] public int DamageInflictedUponLook { get; set; }
    
    public override bool HandleEvent(LookedAtEvent e) 
    {
        GameManager.Instance.ThePlayer.GetComponent<Health>().HealthCurrent -= DamageInflictedUponLook;
        return true; 
    }
}