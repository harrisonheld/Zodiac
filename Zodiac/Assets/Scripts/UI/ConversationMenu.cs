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
        [SerializeField] 

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
        }
    }
}
