using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PickObjectMenu : MonoBehaviour, IZodiacMenu
{
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }
    
    private List<GameObject> candidates;

    public void RefreshUI()
    {
    }
    public void GainFocus()
    {
    }
    private void Clear()
    {
    }

    /// <summary>
    /// Allow the user to select any amount of entities (including only one, or even selecting none) from a given list of entities.
    /// </summary>
    /// <param name="candidates">The list of entities to be selected from.</param>
    /// <returns></returns>
    public List<Item> SelectMultiple(List<GameObject> _candidates)
    {
        List<Item> selected = new();

        candidates = _candidates;
        
        Common.menuManager.Open(this);

        // wait for the user to select items
        while(!ZodiacInput.InputMap.UI.Submit.triggered)
        {
            Debug.Log("wee");
        }

        return selected;
    }

}
