using System.Collections.Generic;
using UnityEngine;

public class TargetEntity : ITargetingMechanism
{
    private GameObject _entity = null;

    public TargetEntity()
    {
        _entity = null;
    }

    public bool HandleInput(ZodiacInputMap inputMap)
    {
        return false;
    }

    public GameObject GetTargettedEntity()
    {
        return _entity;
    }
}