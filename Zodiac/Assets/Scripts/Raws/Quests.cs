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
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new QuestStepConverter());
                List<Quest> questsInFile = JsonConvert.DeserializeObject<List<Quest>>(json, settings);

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

        private class QuestStepConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(QuestStepBase);
            }
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);

                if (jsonObject["Type"] == null)
                    throw new JsonSerializationException("Quest type not specified.");

                string questType = jsonObject["Type"].Value<string>();
                if (questType == "SpeakToNpc")
                {
                    return jsonObject.ToObject<QuestStepSpeakToNpc>();
                }

                throw new JsonSerializationException($"Could not find the quest type '{questType}'.");
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}