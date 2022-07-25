using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyHaver : MonoBehaviour
{
    [SerializeField] public int Speed = 1000; // how much energy is recovered per round
    [SerializeField] public int Energy = 0;

    public bool OutOfEnergy()
    {
        return Energy <= 0;
    }
}