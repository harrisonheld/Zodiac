using System;
using UnityEngine;

class SerializeInteraction : IInteraction
{
    public string Name => "Serialize Player";
    public void Perform()
    {
        Serialization.SerializeEntity(GameManager.Instance.ThePlayer, @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Entities\test.xml");
    }
}