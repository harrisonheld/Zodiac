using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MissileWeapon : ZodiacComponent
{
    [SerializeField] public int Damage { get; set; } = 1;
    [SerializeField] public int AttackCost { get; set; } = 1000;
    [SerializeField] public AmmoType AmmoType { get; set; }
}