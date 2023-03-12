using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Equippable : ZodiacComponent
{
    [field: SerializeField] public SlotType SlotType { get; set; }
}