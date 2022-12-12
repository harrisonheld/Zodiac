using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : ZodiacComponent
{
    [SerializeField] public int HealthMax { get; set; } = 10;
    [SerializeField] public int Defense { get; set; } = 0;
    private int healthCurrent = 10;
    [SerializeField] public int HealthCurrent { get => healthCurrent;
        set {
            healthCurrent = value;
            if (healthCurrent <= 0)
            {   
                GameManager.Instance.BreakEntity(this.gameObject);
            }
        } 
    }
}
