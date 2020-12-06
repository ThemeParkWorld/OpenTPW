using Engine.Utils.DebugUtils;
using Newtonsoft.Json;

namespace Engine.Utils
{
    struct GameSettingsValues
    {
        #pragma warning disable CS0649 // Field is never assigned to, and will always have its default value 'null'
        public int gameResolutionX;
        public int gameResolutionY;
        public bool vsyncEnabled;
        public int framerateLimit;
        public int gamePosX;
        public int gamePosY;
        public int monitor;
        public bool fullscreen;
        public string rconPassword;
        public bool rconEnabled;
        public int rconPort;
        public int webPort;
        public float physTimeStep;
        public float updateTimeStep;
        public string editorTheme;

        // TODO: cascades
        public int shadowMapX;
        public int shadowMapY;
        #pragma warning restore CS0649
    }

    public sealed class GameSettings
    {
        private static GameSettingsValues values;

        public static int GameResolutionX { get => values.gameResolutionX; set => values.gameResolutionX = value; }
        public static int GameResolutionY { get => values.gameResolutionY; set => values.gameResolutionY = value; }
        public static bool VsyncEnabled { get => values.vsyncEnabled; set => values.vsyncEnabled = value; }
        public static int FramerateLimit { get => values.framerateLimit; set => values.framerateLimit = value; }
        public static int GamePosX { get => values.gamePosX; set => values.gamePosX = value; }
        public static int GamePosY { get => values.gamePosY; set => values.gamePosY = value; }
        public static int Monitor { get => values.monitor; set => values.monitor = value; }
        public static bool Fullscreen { get => values.fullscreen; set => values.fullscreen = value; }
        public static string RconPassword { get => values.rconPassword; set => values.rconPassword = value; }
        public static bool RconEnabled { get => values.rconEnabled; set => values.rconEnabled = value; }
        public static int RconPort { get => values.rconPort; set => values.rconPort = value; }
        public static int WebPort { get => values.webPort; set => values.webPort = value; }
        public static float PhysTimeStep { get => values.physTimeStep; set => values.physTimeStep = value; }
        public static float UpdateTimeStep { get => values.updateTimeStep; set => values.updateTimeStep = value; }
        public static int ShadowMapX { get => values.shadowMapX; set => values.shadowMapX = value; }
        public static int ShadowMapY { get => values.shadowMapY; set => values.shadowMapY = value; }
        public static string EditorTheme { get => values.editorTheme; set => values.editorTheme = value; }

        public static void LoadValues()
        {
            Logging.Log("Loading game settings");
            var fileContents = System.IO.File.ReadAllText("GameSettings.json");
            Logging.Log(fileContents);
            values = JsonConvert.DeserializeObject<GameSettingsValues>(fileContents);
        }

        public static void SaveValues()
        {
            Logging.Log("Saving game settings");
            var fileContents = JsonConvert.SerializeObject(values);
            Logging.Log(fileContents);
            System.IO.File.WriteAllText("GameSettings.json", fileContents);
        }
    }
}
