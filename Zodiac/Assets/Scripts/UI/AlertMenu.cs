using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AlertMenu : MonoBehaviour, IZodiacMenu
{
    [SerializeField] TextMeshProUGUI textmesh;
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

    public static AlertMenu Instance { get; private set; }
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

    public void RefreshUI() { }
    public void GainFocus() { }

    public void ShowText(string _text)
    {
        textmesh.text = _text;
        MenuManager.Instance.Open(this);
    }
}
