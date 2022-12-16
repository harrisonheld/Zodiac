using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Equippable : ZodiacComponent
{
    [field: SerializeField] public SlotType slotType { get; set; }
}

public enum SlotType
{
    Hand,
    Missile, // for your ranged weapon
    Head,
    Body,
    Legs
}