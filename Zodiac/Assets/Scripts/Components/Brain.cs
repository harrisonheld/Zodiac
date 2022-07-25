﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Brain : MonoBehaviour
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
}