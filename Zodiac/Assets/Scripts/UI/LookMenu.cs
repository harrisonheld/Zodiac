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
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI body;

    public void RefreshUI()
    {
        if(subject == null)
        {
            title.text = "";
            body.text = "";
            healthText.text = "";
            return;
        }

        Visual vis = subject.GetComponent<Visual>();
        title.text = vis.DisplayName;
        body.text = vis.Description;

        Health health = subject.GetComponent<Health>();
        int healthCurr = health.HealthCurrent;
        int healthMax = health.HealthMax;
        float percent = 100.0f * (float)healthCurr / (float)healthMax;
        healthText.text = $"{healthCurr} / {healthMax} ({percent}%)";
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
