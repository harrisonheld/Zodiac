using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Linq;

public class PickMenu : MonoBehaviour, IZodiacMenu
{
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

    [SerializeField] GameObject optionsContainer;
    [SerializeField] GameObject buttonPrefab;

    private IList _options;
    private Action<object> _action;
    private Func<object, string> _getName;

    private string _prompt = "Pick";
    private bool _closeOnPick = true;
    public static PickMenu Instance { get; private set; }
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
        Clear();

        for (int i = 0; i < _options.Count; i++)
        {
            GameObject optionButton = Instantiate(buttonPrefab);
            TextMeshProUGUI label = optionButton.GetComponentInChildren<TextMeshProUGUI>();
            label.text = _getName(_options[i]);
            optionButton.transform.SetParent(optionsContainer.transform, false);

            var option = _options[i];
            int optionIdx = i;
            
            optionButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                _options.Remove(option);
                _action(option);

                if(_closeOnPick)
                {
                    MenuManager.Instance.Close(this);
                    return;
                }
                if (_options.Count == 0)
                {
                    MenuManager.Instance.Close(this);
                    return;
                }

                RefreshUI();

                // select next option after removal
                int nextOptionIdx = optionIdx;
                if (nextOptionIdx == _options.Count)
                    nextOptionIdx--;
                optionsContainer.transform.GetChild(nextOptionIdx).GetComponent<Selectable>().Select();
            });
        }
    }
    public void GainFocus()
    {
        if (optionsContainer.transform.childCount > 0)
        {
            optionsContainer.transform.GetChild(0).GetComponent<Selectable>().Select();
        }
    }
    private void Clear()
    {
        // mark all children for destruction
        foreach (Transform child in optionsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        // detach children now, as they may not be destroyed instantly
        optionsContainer.transform.DetachChildren();
    }

    /// <summary>
    /// Bring up the UI to pick a single item from a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options">A list of items to choose from.</param>
    /// <param name="callback">An action to be done to the item once chosen.</param>
    /// <returns></returns>
    public void PickOne<T>(IList<T> options, Func<T, string> getName, Action<T> action, string prompt = "Pick", bool closeOnPick = true)
    {
        // convert to more generic types
        _options = options.Cast<object>().ToList();
        _action = pickable => action((T)pickable);
        _getName = obj => getName((T)obj);

        _prompt = prompt;
        _closeOnPick = closeOnPick;

        RefreshUI();
        MenuManager.Instance.Open(this);
    }
}
