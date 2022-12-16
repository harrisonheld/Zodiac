using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : ZodiacComponent
{
    [field: SerializeField] public AmmoType AmmoType { get; set; }
}

public enum AmmoType
{
    Bullet,
    Arrow
}
