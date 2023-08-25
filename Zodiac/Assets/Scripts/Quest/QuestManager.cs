using System.Collections.Generic;
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

        private List<Quest> _activeQuests = new List<Quest>();

        public void AddQuest(string questId)
        {
            Quest quest = Raws.Quests.FromId(questId);
            _activeQuests.Add(quest);
        }

        public void CompleteQuestStep(string QuestId, string QuestStepId)
        {
            return;
        }
    }
}