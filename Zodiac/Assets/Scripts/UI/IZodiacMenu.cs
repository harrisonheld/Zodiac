using System;
using UnityEngine;
using UnityEngine.UI;

public interface IZodiacMenu
{
    public static IZodiacMenu Instance { get; private set; }
    public Canvas Canvas { get; }
    public CanvasGroup CanvasGroup { get; }
    public void RefreshUI();
    public void GainFocus();
}
