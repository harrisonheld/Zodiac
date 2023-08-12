using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// entites that equip this item will get a stat bonus
public class StatModWhileEquipped : ZodiacComponent
{
    [field: SerializeField] public string StatType { get; set; }
    [field: SerializeField] public int Bonus { get; set; }

    public override string GetDescription()
    {
        if(Bonus > 0)
            return $"+{Bonus} {StatType} while equipped.";
        else
            return $"-{-Bonus} {StatType} while equipped.";
    }
}