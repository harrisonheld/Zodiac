using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PhysicalAttributes : ZodiacComponent
{
    [SerializeField] public bool Solid = false; // can other entities walk through this?
    [SerializeField] public bool OccludesVison = false; // should this block line of sight?
    [SerializeField] public int Weight = 100; // weight in pounds
}
