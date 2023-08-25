using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using QuestNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Raws
{
    public static class Quests
    {
        private const string QUESTS_DIR = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Raws\Quests\";

        private static Dictionary<string, Quest> _quests = new();
        private static bool _initialized = false;
        public static void Initialize()
        {
            _quests.Clear();

            // load all loot tables from file
            foreach (string file in Directory.GetFiles(QUESTS_DIR))
            {
                if (!file.EndsWith(".json"))
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(file);
                string filePath = Path.Combine(QUESTS_DIR, file);

                string json = File.ReadAllText(filePath);
                List<Quest> questsInFile = JsonConvert.DeserializeObject<List<Quest>>(json);

                foreach (Quest quest in questsInFile)
                {
                    _quests.Add(quest.Id, quest);
                }
            }

            _initialized = true;
        }

        public static Quest FromId(string questId)
        {
            if (!_initialized)
                Initialize();

            return _quests[questId];
        }
    }
}