using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Raws;
using UnityEditor.Experimental.GraphView;

namespace UI
{
    class ConversationMenu : MonoBehaviour, IZodiacMenu
    {
        public Canvas Canvas { get => GetComponent<Canvas>(); }
        public GameObject GameObject { get => gameObject; }
        public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

        private ConversationNode _currentNode;

        [SerializeField] TextMeshProUGUI npcText;
        [SerializeField] Image npcPortrait;
        [SerializeField] TextMeshProUGUI npcNametag;
        [SerializeField] GameObject choiceContainer;
        [SerializeField] GameObject choicePrefab;

        public void RefreshUI()
        {
            npcText.text = _currentNode.NpcText;
        }
        public void GainFocus()
        {
        }
        public void SetConversation(string conversationNodeId)
        {
            _currentNode = Conversations.ById(conversationNodeId);

            foreach(string choice in _currentNode.Options)
            {
                ConversationNode choiceNode = Conversations.ById(choice);

                GameObject choiceObject = Instantiate(choicePrefab);
                choiceObject.transform.SetParent(choiceContainer.transform, false);
                choiceObject.GetComponent<ConversationChoice>().SetChoice(choiceNode);
            }
        }
        public void SetSpeaker(GameObject speaker)
        {
            npcPortrait.sprite = speaker.GetComponent<SpriteRenderer>().sprite;
            npcNametag.text = speaker.GetComponent<Visual>().DisplayName;
        }
    }
}
