using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] public int HealthMax = 10;
    [SerializeField] private int healthCurrent = 10;
    public int HealthCurrent { get => healthCurrent;
        set {
            healthCurrent = value;
            if (healthCurrent <= 0)
                GameManager.BreakEntity(this.gameObject);
        } 
    }
    [SerializeField] public int Defense = 0;
}
