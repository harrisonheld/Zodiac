using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Brain : ZodiacComponent
{
    [SerializeField] public AiType Ai { get; set; }
    [SerializeField] public GameObject Target { get; set; }
}

public enum AiType
{
    Inert,
    Wanderer,
    Seeker,
    Projectile, // arrows
}