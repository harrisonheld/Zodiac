using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Brain : ZodiacComponent
{
    [SerializeField] public AiType Ai;
    [SerializeField] public GameObject Target;
}

public enum AiType
{
    Inert,
    PlayerControlled,
    Wanderer,
    Seeker,
    Projectile, // arrows
}