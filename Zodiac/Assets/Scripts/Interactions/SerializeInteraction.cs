using System;
using UnityEngine;

class SerializeInteraction : IInteraction
{
    public string Name => "Serialize Scene";
    public void Perform()
    {
        EntitySerializer serializer = new EntitySerializer();
        serializer.SerializeScene(@"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Entities\serialize_scene_test2.xml");
    }
}