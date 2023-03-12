using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : ZodiacComponent
{
    [field: SerializeField] public SlotType Type; // what type of Equippable can be in this slot
    [field: SerializeField] public GameObject Contained; // what Equippable is in this slot

    public bool IsEmpty()
    {
        return Contained == null;
    }
}
