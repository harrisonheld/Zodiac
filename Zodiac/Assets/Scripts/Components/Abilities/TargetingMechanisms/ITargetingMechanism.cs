using System.Collections.Generic;
using UnityEngine;

public interface ITargetingMechanism
{
    // return true when the target selection is done
    public bool HandleInput();
}