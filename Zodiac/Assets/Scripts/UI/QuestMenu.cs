using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using System.Text;
using QuestNamespace;

namespace UI
{
    class QuestMenu : MonoBehaviour, IZodiacMenu
    {
        [SerializeField] TextMeshProUGUI text;
        public Canvas Canvas { get => GetComponent<Canvas>(); }
        public GameObject GameObject { get => gameObject; }
        public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

        public static QuestMenu Instance { get; private set; }
        public void Awake()
        {
            if (Instance == null) // If there is no instance already
            {
                DontDestroyOnLoad(this.gameObject); // Keep the GameObject, this component is attached to, across different scenes
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject); // Destroy the GameObject, this component is attached to
            }
        }

        public void RefreshUI()
        {
            StringBuilder sb = new();

            QuestManager.Instance.GetActiveQuests().ForEach(quest =>
            {
                sb.AppendLine(quest.Title);
                sb.AppendLine(quest.Subtitle);
                quest.Steps.ForEach(step =>
                {
                    sb.Append(step.IsComplete ? '+' : '-'); sb.Append('\t'); sb.Append(step.Description);
                    sb.AppendLine();
                });
            });

            text.text = sb.ToString();
        }
        public void GainFocus() { }
    }
}