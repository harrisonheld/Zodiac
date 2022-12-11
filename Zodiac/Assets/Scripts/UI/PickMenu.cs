using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PickMenu : MonoBehaviour, IZodiacMenu
{
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

    [SerializeField] GameObject optionsContainer;
    [SerializeField] GameObject buttonPrefab;

    private List<GameObject> options; // the items to be picked from

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

        for (int i = 0; i < options.Count; i++)
        {
            GameObject optionButton = Instantiate(buttonPrefab);
            TextMeshProUGUI label = optionButton.GetComponentInChildren<TextMeshProUGUI>();
            label.text = options[i].GetComponent<Visual>().DisplayName;
            optionButton.transform.SetParent(optionsContainer.transform, false);

            GameObject option = options[i];
            int optionIdx = i;
            
            optionButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                options.Remove(option);
                GameManager.Pickup(GameManager.Instance.ThePlayer, option.GetComponent<Item>());

                if (options.Count == 0)
                {
                    MenuManager.Instance.Close(this);
                    return;
                }

                RefreshUI();

                // select next option after removal
                int nextOptionIdx = optionIdx;
                if (nextOptionIdx == options.Count)
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



    public void ShowGetMenu(List<GameObject> candidates)
    {
        options = candidates;
        RefreshUI();
        MenuManager.Instance.Open(this);
    }
}
