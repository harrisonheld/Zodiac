using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LookMenu : MonoBehaviour, IZodiacMenu
{
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI body;

    public void RefreshUI()
    {
        if(subject == null)
        {
            title.text = "";
            body.text = "";
            return;
        }
        Visual vis = subject.GetComponent<Visual>();
        title.text = vis.DisplayName;
        body.text = vis.Description;

    }
    public void GainFocus()
    {
    }

    private GameObject subject; // the thing we are looking at

    public void SetSubject(GameObject newSubject)
    {
        subject = newSubject;
        RefreshUI();
    }
}
