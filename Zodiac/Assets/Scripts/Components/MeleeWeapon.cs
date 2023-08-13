using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MeleeWeapon : ZodiacComponent
{
    [field: SerializeField] public string Damage { get; set; } = "1d6";
    [field: SerializeField] public int AttackCost { get; set; } = 1000;
}