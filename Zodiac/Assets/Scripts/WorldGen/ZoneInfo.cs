using System;

namespace WorldGen
{
    internal class ZoneInfo
    {
        // variable size zones! not just a caves of qud rip off now, huh?
        public int Width { get; set; }
        public int Height { get; set; }
        public ITerrainGenerator Generator { get; set; }
        public Random ZoneRandom { get; set; }
        public Gaps Gaps { get; set; }
    }
}