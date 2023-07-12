using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

[DisallowMultipleComponent]
public class LookingHurts : ZodiacComponent
{
    [field: SerializeField] public int DamageInflictedUponLook { get; set; }
    
    public override bool HandleEvent(LookedAtEvent e) 
    {
        GameManager.Instance.ThePlayer.GetComponent<Health>().HealthCurrent -= DamageInflictedUponLook;
        MenuManager.Instance.Log($"You take {DamageInflictedUponLook} damage from looking at the {gameObject.GetComponent<Visual>().DisplayName}.");
        return true; 
    }

    public override string GetDescription()
    {
        return $"Looking at this inflicts {DamageInflictedUponLook} damage to the looker.";
    }
}