using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Raws;
using UnityEditor.Experimental.GraphView;
using UnityEngine.InputSystem;

namespace UI
{
    class ConversationMenu : MonoBehaviour, IZodiacMenu
    {
        public Canvas Canvas { get => GetComponent<Canvas>(); }
        public GameObject GameObject { get => gameObject; }
        public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

        private Stack<string> _conversationNodes = new Stack<string>(); // to keep track of prior nodes so we can go back
        private GameObject _speaker;

        [SerializeField] TextMeshProUGUI npcText;
        [SerializeField] Image npcPortrait;
        [SerializeField] TextMeshProUGUI npcNametag;
        [SerializeField] TextMeshProUGUI helpText;
        [SerializeField] GameObject choiceContainer;
        [SerializeField] GameObject choicePrefab;

        public void Update()
        {
            if(ZodiacInput.InputMap.UI.Backspace.triggered)
            {
                ZodiacInput.InputMap.UI.Backspace.Reset();
                GoBackANode();
            }
        }

        private void Clear()
        {
            // mark all children for destruction
            foreach (Transform child in choiceContainer.transform)
            {
                Destroy(child.gameObject);
            }
            // detach children now, as they may not be destroyed instantly
            choiceContainer.transform.DetachChildren();

            npcText.text = "[placeholder text]";
            npcPortrait.sprite = null;
            npcNametag.text = "[placeholder name]";
        }
        public void RefreshUI()
        {
            Clear();
            helpText.text = $"Made a mistake? Press '{ZodiacInput.InputMap.UI.Backspace.GetBindingDisplayString()}' to take back what you said.";

            string currentNodeId = _conversationNodes.Peek();
            ConversationNode currentNode = Conversations.ById(currentNodeId);

            if (currentNode != null)
            {
                npcText.text = currentNode.NpcText;
            }

            if (_speaker != null)
            {
                npcPortrait.sprite = _speaker.GetComponent<SpriteRenderer>().sprite;
                npcNametag.text = _speaker.GetComponent<Visual>().DisplayName;
                npcPortrait.color = Color.white;
            }

            if(!string.IsNullOrEmpty(currentNode.NpcPortrait))
            {
                Texture2D portrait = Resources.Load<Texture2D>($"Portraits/{currentNode.NpcPortrait}");
                portrait.filterMode = FilterMode.Point; // for pixelated look
                npcPortrait.sprite = Sprite.Create(portrait, new Rect(0, 0, portrait.width, portrait.height), Vector2.one * 0.5f);
                npcPortrait.color = new Color(0.5f, 0.5f, 0.75f);
            }

            for(int i = 0; i < currentNode.Options.Count; i++)
            {
                string choice = currentNode.Options[i];
                ConversationNode choiceNode = Conversations.ById(choice);

                GameObject choiceObject = Instantiate(choicePrefab);
                choiceObject.transform.SetParent(choiceContainer.transform, false);
                choiceObject.GetComponent<ConversationChoice>().SetChoice(choiceNode);

                Button choiceButton = choiceObject.GetComponent<Button>();
                choiceButton.onClick.AddListener(() =>
                {
                    GoToNode(choiceNode.Id);
                    RefreshUI();
                });

                if(i==0)
                {
                    choiceButton.Select();
                }
            }
        }
        public void GainFocus()
        {
        }
        public void GoToNode(string conversationNodeId)
        {
            _conversationNodes.Push(conversationNodeId);
            RefreshUI();
        }
        public void GoBackANode()
        {
            if(_conversationNodes.Count > 0)
            {
                _conversationNodes.Pop();
                RefreshUI();
            }
        }
        public void SetSpeaker(GameObject speaker)
        {
            _speaker = speaker;
            RefreshUI();
        }
    }
}
