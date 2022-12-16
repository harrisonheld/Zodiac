using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Brain : ZodiacComponent
{
    [field: SerializeField] public AiType Ai { get; set; }
    [field: SerializeField] public GameObject Target { get; set; }
}

public enum AiType
{
    Inert,
    Wanderer,
    Seeker,
    Projectile, // arrows
}