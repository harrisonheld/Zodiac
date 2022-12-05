using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MissileWeapon : ZodiacComponent
{
    [SerializeField] public int Damage = 1;
    [SerializeField] public int AttackCost = 1000;
    [SerializeField] public AmmoType AmmoType;
}