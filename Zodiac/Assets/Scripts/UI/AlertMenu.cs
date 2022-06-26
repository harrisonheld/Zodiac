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

    public void RefreshUI() { }
    public void GainFocus() { }

    public void SetText(string _text)
    {
        textmesh.text = _text;
    }
}
