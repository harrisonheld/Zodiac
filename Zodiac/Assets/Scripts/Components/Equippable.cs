using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Equippable : MonoBehaviour
{
    [SerializeField] public SlotType slotType;
}

public enum SlotType
{
    Hand,
    Missile, // for your ranged weapon
    Head,
    Body,
    Legs
}