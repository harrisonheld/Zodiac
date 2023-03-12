using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stat : ZodiacComponent
{
    public StatType StatType { get; set; } = StatType.Vigor;
    public int BaseValue { get; set; } = 10;
    
    public int GetEffectiveValue()
    {
        // modifications here
        return BaseValue;
    }
}