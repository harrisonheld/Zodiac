using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PhysicalAttributes : ZodiacComponent
{
    [SerializeField] public bool Solid { get; set; } = false; // can other entities walk through this?
    [SerializeField] public bool OccludesVison { get; set; } = false; // should this block line of sight?
    [SerializeField] public int Weight { get; set; } = 100; // weight in pounds
}
