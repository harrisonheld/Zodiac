using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] public int MaxHealth = 10;
    [SerializeField] private int currentHealth = 10;
    public int CurrentHealth { get => currentHealth;
        set {
            currentHealth = value;
            if (currentHealth <= 0)
                GameManager.BreakEntity(this.gameObject);
        } 
    }
    [SerializeField] public int Defense = 0;
}
