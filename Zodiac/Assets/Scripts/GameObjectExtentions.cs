using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GameObjectExtentions {
    public static void FireEvent(this GameObject go, ZodiacEvent e)
    {
        foreach (ZodiacComponent comp in go.GetComponents<ZodiacComponent>())
        {
            comp.HandleEvent(e);
        }
    }

    public static List<IInteraction> GetInteractions(this GameObject go)
    {
        // using linq, get all the interactions in all the components of this gameobject
        return go.GetComponents<ZodiacComponent>()
            .SelectMany(comp => comp.GetInteractions())
            .ToList();
    }

    public static int GetGUID(this GameObject go)
    {
        return 1;
    }
}
