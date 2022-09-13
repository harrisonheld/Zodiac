using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour
{
    public AmmoType AmmoType;
}

public enum AmmoType
{
    Bullet,
    Arrow
}
