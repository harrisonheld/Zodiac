using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnergyHaver : ZodiacComponent
{
    [SerializeField] public int Quickness = 1000; // how much energy is recovered per round, as well as the cap of how much energy the entity can have
    [SerializeField] public int Energy = 1000;
}