using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : ZodiacComponent
{
    [field: SerializeField] public SlotType SlotType { get; set; } // what type of Equippable can be in this slot
    [field: SerializeField] public GameObject Contained { get; set; } // what Equippable is in this slot

    public bool IsEmpty()
    {
        return Contained == null;
    }
}

public enum SlotType
{
    Hand,
    Missile, // for your ranged weapon
    Head,
    Body,
    Legs
}