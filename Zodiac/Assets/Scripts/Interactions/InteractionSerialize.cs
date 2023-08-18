using System;
using UnityEngine;

class InteractionSerialize : IInteraction
{
    public string Name => "Serialize Scene";
    public void Perform()
    {
        Serializer serializer = new Serializer();
        serializer.SerializeScene(@"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Entities\serialize_scene_test2.xml");
    }
}