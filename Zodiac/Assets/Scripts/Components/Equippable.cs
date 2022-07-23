using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equippable : MonoBehaviour
{
    [SerializeField] public SlotType slotType;
}

public enum SlotType
{
    Hand,
    Head,
    Body,
    Legs
}