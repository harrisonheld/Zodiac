using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseStats : ZodiacComponent
{
    [field: SerializeField] public Dictionary<string, int> Stats { get; set; } = new Dictionary<string, int>();
}

public class Stats
{
    private Dictionary<string, int> _stats;

    public Stats()
    {
        _stats = new();
    }

    // indexer lets you access stats that aren't in the dictionary, and add them automatically
    public int this[string key]
    {
        get
        {
            if (_stats.ContainsKey(key))
                return _stats[key];
            else
                return 0;
        }
        set
        {
            if (_stats.ContainsKey(key))
                _stats[key] = value;
            else
                _stats.Add(key, value);
        }
    }
}