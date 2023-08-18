using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class BaseStats : ZodiacComponent
{
    [field: SerializeField] public Dictionary<string, int> Stats { get; set; } = new Dictionary<string, int>();

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(Stats.Count);
        foreach (var stat in Stats)
        {
            writer.Write(stat.Key);
            writer.Write(stat.Value);
        }
    }
    public override void Deserialize(BinaryReader reader, Dictionary<int, GameObject> idToEntity = null)
    {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            string key = reader.ReadString();
            int value = reader.ReadInt32();
            Stats.Add(key, value);
        }
    }
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

    public int GetMod(string stat)
    {
        int statValue = this[stat];
        return (statValue - 10) / 2;
    }
}