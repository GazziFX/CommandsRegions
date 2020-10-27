using Rocket.API;

namespace CommandsRegions
{
    public class Config : IRocketPluginConfiguration, IDefaultable
    {
        public void LoadDefaults()
        {
            Locations = new Location[]
            {
                new Location
                {
                    Radius = 5.5f,
                    Commands = new string[]
                    {
                        "home",
                        "tpa"
                    }
                }
            };
        }

        public Location[] Locations;
    }

    public struct Location
    {
        public float X;
        public float Z;
        public float Radius;
        public bool IsWhitelist;
        public string[] Commands;
    }
}
