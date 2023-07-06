using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stat : ZodiacComponent
{
    [field: SerializeField] public StatType StatType { get; set; } = StatType.Vigor;
    [field: SerializeField] public int BaseValue { get; set; } = 10;
    
    public int EffectiveValue()
    {
        // modifications here
        return BaseValue;
    }
}

public enum StatType
{
    Vigor,
    Prowess,
    Arcane,
    Dexterity,
}