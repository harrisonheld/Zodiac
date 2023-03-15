using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : ZodiacComponent
{
    [field: SerializeField] public int HealthMax { get; set; } = 10;
    [field: SerializeField] public int Defense { get; set; } = 0;
    [SerializeField] private int healthCurrent = 10;
    public int HealthCurrent { 
        get => healthCurrent;
        set 
        {
            healthCurrent = value;
            if (healthCurrent <= 0)
            {   
                GameManager.Instance.BreakEntity(this.gameObject);
            }
        } 
    }

    public string GetHealthString()
    {
        return $"{HealthCurrent} / {HealthMax}";
    }
}
