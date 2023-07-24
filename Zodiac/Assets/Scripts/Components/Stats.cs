using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stats : ZodiacComponent
{
    public int Level { get; set; } = 1;
    public int XP { get; set; } = 0;
    [field: SerializeField] public Dictionary<StatType, int> BaseStats { get; set; } = new Dictionary<StatType, int>();
}

public enum StatType
{
    Prowess,
    Vigor,
    Dexterity,
    Arcane,
}