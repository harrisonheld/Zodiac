using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LogMenu : MonoBehaviour, IZodiacMenu
{
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI statsText;


    public static LogMenu Instance { get; private set; }
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

    public void OnGUI()
    {
        int padding = 20;
        
        Health health = GameManager.Instance.ThePlayer.GetComponent<Health>();
        healthText.text = "HP: " + health.HealthCurrent + " /" + health.HealthMax + "\n" +
            "DP: " + health.Defense;

        string stats = "";
        foreach (Stat stat in GameManager.Instance.ThePlayer.GetComponents<Stat>())
        {
            stats += (stat.StatType.ToString() + ": ").PadRight(padding) + stat.GetEffectiveValue() + "\n";
        }
        statsText.text = stats;
    }

    public void RefreshUI() { }
    public void GainFocus() { }

    public void Log(string text)
    {
        Debug.Log(text);
    }
}
