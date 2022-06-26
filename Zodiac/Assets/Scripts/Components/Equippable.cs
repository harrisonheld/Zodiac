using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equippable : MonoBehaviour
{
    [SerializeField] public Slot slot;
}

public enum Slot
{
    Head,
    Body,
    Legs
}