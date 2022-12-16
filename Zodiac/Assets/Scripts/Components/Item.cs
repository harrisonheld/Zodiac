using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Item : ZodiacComponent
{
    [field: SerializeField] public int Count { get; set; } = 1; // how many are in the stack
}
