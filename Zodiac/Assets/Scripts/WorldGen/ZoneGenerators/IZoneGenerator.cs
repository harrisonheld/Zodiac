using WorldGen;

namespace WorldGen
{
    public interface IZoneGenerator
    {
        public ZoneInfo Generate(System.Random random, Gaps gaps);
    }
}