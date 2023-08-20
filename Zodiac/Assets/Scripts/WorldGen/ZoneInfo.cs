using System;
using System.IO;

namespace WorldGen
{
    public class ZoneInfo
    {
        // variable size zones! not just a caves of qud rip off now, huh?
        public int Width { get; set; }
        public int Height { get; set; }
        // position in the world
        public int X { get; set; }
        public int Y { get; set; }
        
        // name of the biome
        public string BiomeId { get; set; }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(X);
            writer.Write(Y);
        }
        public void Deserialize(BinaryReader reader)
        {
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
        }
    }
}