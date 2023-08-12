using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GameObjectExtentions 
{
    public static void FireEvent(this GameObject go, ZodiacEvent e)
    {
        foreach (ZodiacComponent comp in go.GetComponents<ZodiacComponent>())
        {
            comp.HandleEvent(e);
        }
    }

    public static List<IInteraction> GetInteractions(this GameObject go)
    {
        // get all the interactions in all the components of this gameobject
        return go.GetComponents<ZodiacComponent>()
            .SelectMany(comp => comp.GetInteractions())
            .ToList();
    }


    public static Dictionary<string, int> GetEffectiveStats(this GameObject entity)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();

        // deep clone base stats
        Dictionary<string, int> baseStats = entity.GetComponent<BaseStats>().Stats;
        if (baseStats != null)
        {
            foreach (KeyValuePair<string, int> pair in baseStats)
                result.Add(pair.Key, pair.Value);
        }

        // add equipment stats
        foreach (Slot slot in entity.GetComponents<Slot>())
        {
            if (slot.Contained == null)
                continue;

            var equipmentStatMods = slot.Contained.GetComponents<StatModWhileEquipped>();
            foreach (StatModWhileEquipped statMod in equipmentStatMods)
            {
                string stat = statMod.StatType;
                int mod = statMod.Bonus;

                if (result.ContainsKey(stat))
                    result[stat] += mod;
                else
                    result.Add(stat, mod);
            }
        }

        return result;
    }
}
