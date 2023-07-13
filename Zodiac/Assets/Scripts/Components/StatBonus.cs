using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBonus : ZodiacComponent
{
    [field: SerializeField] public StatType StatType { get; set; }
    [field: SerializeField] public int Bonus { get; set; }

    public override string GetDescription()
    {
        if(Bonus > 0)
            return $"Equipping this item will add {Bonus} to {StatType}.";
        else
            return $"Equipping this item will subtract {-Bonus} from {StatType}.";
    }
}