using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PhysicalAttributes : ZodiacComponent
{
    [field: SerializeField] public bool Solid { get; set; } = false; // can other entities walk through this?
    [field: SerializeField] public bool OccludesVison { get; set; } = false; // should this block line of sight?
    [field: SerializeField] public int Weight { get; set; } = 100; // weight in pounds
}
