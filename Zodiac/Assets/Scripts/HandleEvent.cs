using System;
using UnityEngine;

public static class GameObjectExtentions {
    public static void FireEvent(this GameObject go, ZodiacEvent e)
    {
        foreach (ZodiacComponent comp in go.GetComponents<ZodiacComponent>())
        {
            comp.HandleEvent(e);
        }
    }
}
