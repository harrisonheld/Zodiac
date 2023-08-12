using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Xml;

/// <summary>
/// Base class for other components to inherit from
/// </summary>
public abstract class ZodiacComponent : MonoBehaviour
{
    public virtual List<IInteraction> GetInteractions() { return new(); }
    public virtual string GetDescription() { return null; }


    public virtual void HandleEvent(ZodiacEvent e)
    {
        // this will call the appropriate HandleEvent for the event type
        dynamic casted = e;
        HandleEvent(casted);
    }
    public virtual bool HandleEvent(PickedUpEvent e) { return false; }
    public virtual bool HandleEvent(LookedAtEvent e) { return false; }
}