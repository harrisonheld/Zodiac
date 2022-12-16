using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnergyHaver : ZodiacComponent
{
    [field: SerializeField] public int Quickness { get; set; } = 1000; // how much energy is recovered per round, as well as the cap of how much energy the entity can have
    [field: SerializeField] public int Energy { get; set; } = 1000;
}