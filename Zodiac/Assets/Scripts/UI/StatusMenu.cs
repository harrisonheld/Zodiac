using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace UI
{
    internal class StatusMenu : MonoBehaviour
    {
        public Canvas Canvas { get => GetComponent<Canvas>(); }
        public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

        private RectTransform rectTransform;

        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI logText;
        public static StatusMenu Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null) // If there is no instance already
            {
                DontDestroyOnLoad(this.gameObject); // Keep the GameObject, this component is attached to, across different scenes
                Instance = this;
                rectTransform = GetComponentInChildren<VerticalLayoutGroup>().GetComponent<RectTransform>();
            }
            else if (Instance != this)
            {
                Destroy(gameObject); // Destroy the GameObject, this component is attached to
            }
        }

        private void OnGUI()
        {
            GameObject player = GameManager.Instance.ThePlayer;
            if (player == null)
            {
                healthText.text = "";
                this.statsText.text = "";
                return;
            }

            Health health = player.GetComponent<Health>();
            healthText.text = "HP: " + health.GetHealthString();

            Stats playerStats = player.GetEffectiveStats();
            string statsText = "";
            string[] statsToDisplay = new string[] { "Prowess", "Dexterity", "Arcane", "Defense"};
            int padding = statsToDisplay.Max(stat => stat.Length) + 2;
            for(int i = 0; i < statsToDisplay.Length; i++)
            {
                string stat = statsToDisplay[i];

                statsText += stat.PadRight(padding, '.') + playerStats[stat] + "\n";
            }
            statsText += "\n";

            this.statsText.text = statsText + "Turn " + GameManager.Instance.GetTurn() + ", " + ZodiacInput.GetInputMode();
        }
        public void SetSize(Vector2 size)
        {
            rectTransform.sizeDelta = size;
        }
        public void Log(string text)
        {
            Debug.Log(text);
            logText.text += $"[TURN {GameManager.Instance.GetTurn()}]:".PadRight(13) +  $"{text}\n";
        }
    }
}
