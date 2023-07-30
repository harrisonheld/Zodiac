using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WorldGen;

namespace Raws
{
    public static class Conversations
    {
        private const string CONVERSATIONS_DIR = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Raws\Conversations\";

        private static Dictionary<string, ConversationNode> _conversationNodes = new();
        private static bool _initialized = false;
        private static void Initialize()
        {
            _initialized = true;

            foreach (string file in Directory.GetFiles(CONVERSATIONS_DIR))
            {
                if (!file.EndsWith(".json"))
                    continue;

                string fullPath = Path.Combine(CONVERSATIONS_DIR, file);

                string json = File.ReadAllText(fullPath);
                List<ConversationNode> nodesInFile = JsonConvert.DeserializeObject<List<ConversationNode>>(json);

                foreach(ConversationNode node in nodesInFile) {
                    _conversationNodes.Add(node.Id, node);
                }
            }
        }

        public static ConversationNode ById(string nodeId)
        {
            if (!_initialized)
                Initialize();

            return _conversationNodes[nodeId];
        }
    }


    public class ConversationNode
    {
        [JsonProperty("nodeId")]
        public string Id { get; set; }
        [JsonProperty("playerText")]
        public string PlayerText { get; set; } = null; // what the player says, shown to the player when they pick it as an option
        [JsonProperty("npcText")]
        public string NpcText { get; set; } // what the npc being spoken too says in response to the player text
        [JsonProperty("options")]
        public List<string> Options { get; set; } = new(); // what nodes the player can pick in response
    }
}