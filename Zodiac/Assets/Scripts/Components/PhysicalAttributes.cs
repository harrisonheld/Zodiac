using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PhysicalAttributes : ZodiacComponent
{
    [field: SerializeField] public bool Solid { get; set; } = true; // can other entities walk through this?
    [field: SerializeField] public bool OccludesVison { get; set; } = true; // should this block line of sight?
    [field: SerializeField] public int Weight { get; set; } = 1000; // weight in pounds
}
