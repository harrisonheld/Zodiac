using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseStats : ZodiacComponent
{
    [field: SerializeField] public Dictionary<string, int> Stats { get; set; } = new Dictionary<string, int>();
}