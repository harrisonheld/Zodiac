using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] public int Damage = 1;
    [SerializeField] public int AttackCost = 1000;
}