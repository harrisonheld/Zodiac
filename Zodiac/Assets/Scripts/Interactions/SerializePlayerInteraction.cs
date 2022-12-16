using System;
using UnityEngine;

class SerializePlayerInteraction : IInteraction
{
    public string Name => "Serialize Player";
    public void Perform()
    {
        Serialization.SerializeToFile(GameManager.Instance.ThePlayer, @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Entities\test.xml");
    }
}