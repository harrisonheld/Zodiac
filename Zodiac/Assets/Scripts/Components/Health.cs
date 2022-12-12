using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : ZodiacComponent
{
    [SerializeField] public int HealthMax = 10;
    [SerializeField] public int Defense = 0;
    [SerializeField] private int healthCurrent = 10;
    public int HealthCurrent { get => healthCurrent;
        set {
            healthCurrent = value;
            if (healthCurrent <= 0)
            {   
                GameManager.Instance.BreakEntity(this.gameObject);

                if (this.gameObject == GameManager.Instance.ThePlayer)
                {
                    MenuManager.Instance.CloseAll();
                    AlertMenu.Instance.ShowText("You have died.");
                }

            }
        } 
    }
}
