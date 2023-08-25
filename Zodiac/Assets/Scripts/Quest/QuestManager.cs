using System.Collections.Generic;
using System.IO;
using System.Linq;
using UI;
using UnityEngine;

namespace QuestNamespace
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one QuestManager in the scene!");
                return;
            }
            Instance = this;
        }

        [SerializeField] private List<Quest> _activeQuests = new List<Quest>();
        public List<Quest> GetActiveQuests() => _activeQuests;

        public void AddQuest(string questId)
        {
            Quest quest = Raws.Quests.FromId(questId);
            _activeQuests.Add(quest);
        }
        public void CompleteQuestStep(string QuestId, string QuestStepId)
        {
            Quest quest = _activeQuests.Find(q => q.Id == QuestId);
            QuestStep step = quest.Steps.Find(s => s.Id == QuestStepId);

            if (step.IsComplete)
            {
                // don't double complete
                Debug.LogWarning($"Tried to complete quest step '{step.Id}', but it is already complete.");
                return;
            }
            step.IsComplete = true;

            MenuManager.Instance.LogQuestStepComplete(quest, step);

            // complete quest if all steps complete
            if (quest.Steps.TrueForAll(s => s.IsComplete))
            {
                quest.IsComplete = true;
                MenuManager.Instance.LogQuestComplete(quest);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(_activeQuests.Count);

            for(int i = 0; i < _activeQuests.Count; i++)
            {
                writer.Write(_activeQuests[i].Id);
                writer.Write(_activeQuests[i].IsComplete);
                writer.Write(_activeQuests[i].Steps.Count);

                for(int j = 0; j < _activeQuests[i].Steps.Count; j++)
                {
                    writer.Write(_activeQuests[i].Steps[j].Id);
                    writer.Write(_activeQuests[i].Steps[j].IsComplete);
                }
            }
        }
        public void Deserialize(BinaryReader reader)
        {
            _activeQuests.Clear();

            int questsCount = reader.ReadInt32();

            for(int i = 0; i < questsCount; i++)
            {
                string questId = reader.ReadString();
                bool questIsComplete = reader.ReadBoolean();

                Quest quest = Raws.Quests.FromId(questId);

                int stepsCount = reader.ReadInt32();
                for(int j = 0; j < stepsCount; j++)
                {
                    string stepId = reader.ReadString();
                    bool stepIsComplete = reader.ReadBoolean();
                    quest.Steps.Where(s => s.Id == stepId).First().IsComplete = stepIsComplete;
                }
            }
        }
    }
}