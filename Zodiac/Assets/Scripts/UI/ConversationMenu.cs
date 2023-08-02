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
        private GameObject _speaker;

        [SerializeField] TextMeshProUGUI npcText;
        [SerializeField] Image npcPortrait;
        [SerializeField] TextMeshProUGUI npcNametag;
        [SerializeField] GameObject choiceContainer;
        [SerializeField] GameObject choicePrefab;

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

            if(_currentNode != null)
            {
                npcText.text = _currentNode.NpcText;
            }

            if (_speaker != null)
            {
                npcPortrait.sprite = _speaker.GetComponent<SpriteRenderer>().sprite;
                npcNametag.text = _speaker.GetComponent<Visual>().DisplayName;
            }

            if(!string.IsNullOrEmpty(_currentNode.NpcPortrait))
            {
                // TODO: move all this shit somewhere else and don't do it on the fly
                Texture2D originalPortrait = Resources.Load<Texture2D>($"Portraits/{_currentNode.NpcPortrait}");

                // scale portrait down
                int SCALED_WIDTH = 128;
                int SCALED_HEIGHT = 128;
                int widthScaleFactor = originalPortrait.width / SCALED_WIDTH;
                int heightScaleFactor = originalPortrait.height / SCALED_HEIGHT;
                Texture2D filteredPortrait = new Texture2D(SCALED_WIDTH, SCALED_HEIGHT, TextureFormat.ARGB32, false);
                filteredPortrait.filterMode = FilterMode.Point; // for pixelated look
                for (int y = 0; y < SCALED_HEIGHT; y++)
                {
                    for (int x = 0; x < SCALED_WIDTH; x++)
                    {
                        int originalX = Mathf.RoundToInt(x * widthScaleFactor);
                        int originalY = Mathf.RoundToInt(y * heightScaleFactor);

                        Color color = originalPortrait.GetPixel(originalX, originalY);

                        filteredPortrait.SetPixel(x, y, color);
                    }
                }

                // apply 4tone monochrome
                Color[] pixels = filteredPortrait.GetPixels();
                Color[] newPixels = new Color[pixels.Length];

                Color black = new Color(0, 0, 0);
                Color darkGray = new Color(0.25f, 0.25f, 0.25f);
                Color lightGray = new Color(0.75f, 0.75f, 0.75f);
                Color white = new Color(1, 1, 1);

                float threshold = 0.25f;

                for (int i = 0; i < pixels.Length; i++)
                {
                    float grayscale = pixels[i].grayscale;

                    if (grayscale < threshold)
                        newPixels[i] = black;
                    else if (grayscale < 2 * threshold)
                        newPixels[i] = darkGray;
                    else if (grayscale < 3 * threshold)
                        newPixels[i] = lightGray;
                    else
                        newPixels[i] = white;
                }
                filteredPortrait.SetPixels(newPixels);

                // Floyd-Steinberg dithering
                for (int y = 0; y < filteredPortrait.height; y++)
                {
                    for (int x = 0; x < filteredPortrait.width; x++)
                    {
                        Color oldColor = filteredPortrait.GetPixel(x, y);
                        Color newColor = newPixels[y * filteredPortrait.width + x];
                        filteredPortrait.SetPixel(x, y, newColor);
                        Color error = oldColor - newColor;

                        if (x < filteredPortrait.width - 1)
                        {
                            filteredPortrait.SetPixel(x + 1, y, filteredPortrait.GetPixel(x + 1, y) + error * 7 / 16f);
                        }

                        if (x > 0 && y < filteredPortrait.height - 1)
                        {
                            filteredPortrait.SetPixel(x - 1, y + 1, filteredPortrait.GetPixel(x - 1, y + 1) + error * 3 / 16f);
                        }

                        if (y < filteredPortrait.height - 1)
                        {
                            filteredPortrait.SetPixel(x, y + 1, filteredPortrait.GetPixel(x, y + 1) + error * 5 / 16f);
                        }

                        if (x < filteredPortrait.width - 1 && y < filteredPortrait.height - 1)
                        {
                            filteredPortrait.SetPixel(x + 1, y + 1, filteredPortrait.GetPixel(x + 1, y + 1) + error * 1 / 16f);
                        }
                    }
                }

                // apply a colored tint
                Color tint = new Color(0.5f, 0.5f, 0.75f, 1f);
                for (int y = 0; y < SCALED_HEIGHT; y++)
                {
                    for (int x = 0; x < SCALED_WIDTH; x++)
                    {
                        Color color = filteredPortrait.GetPixel(x, y);
                        // Adjust t to control the intensity of the tint
                        Color tintedColor = Color.Lerp(color, tint, 0.25f);
                        filteredPortrait.SetPixel(x, y, tintedColor);
                    }
                }

                filteredPortrait.Apply();

                npcPortrait.sprite = Sprite.Create(filteredPortrait, new Rect(0, 0, filteredPortrait.width, filteredPortrait.height), Vector2.one * 0.5f);
            }

            for(int i = 0; i < _currentNode.Options.Count; i++)
            {
                string choice = _currentNode.Options[i];
                ConversationNode choiceNode = Conversations.ById(choice);

                GameObject choiceObject = Instantiate(choicePrefab);
                choiceObject.transform.SetParent(choiceContainer.transform, false);
                choiceObject.GetComponent<ConversationChoice>().SetChoice(choiceNode);

                Button choiceButton = choiceObject.GetComponent<Button>();
                choiceButton.onClick.AddListener(() =>
                {
                    SetConversation(choiceNode.Id);
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
        public void SetConversation(string conversationNodeId)
        {
            _currentNode = Conversations.ById(conversationNodeId);
            RefreshUI();
        }
        public void SetSpeaker(GameObject speaker)
        {
            _speaker = speaker;
            RefreshUI();
        }
    }
}
