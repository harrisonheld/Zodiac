using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LookMenu : MonoBehaviour, IZodiacMenu
{
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

    [SerializeField] private RectTransform panelRectTransform;

    public static LookMenu Instance { get; private set; }
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

    private GameObject subject; // the thing we are looking at

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI body;

    public void RefreshUI()
    {
        if (subject == null)
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
        healthText.text = health.GetHealthString();

        // add flavor texts for components
        foreach (var comp in subject.GetComponents<ZodiacComponent>())
        {
            string desc = comp.GetDescription();
            if (desc != null)
                body.text += "\n\n * " + desc;
        }

        // add list of equipment
        Slot[] slots = subject.GetComponents<Slot>();
        string eqDesc = slots.Aggregate("", 
            (string str, Slot s) => { return (s.Contained ? str + s.Contained.GetComponent<Visual>().DisplayName + "," : str); }, 
            (string str) => { return str.TrimEnd(','); 
        });
        if (eqDesc != "")
            eqDesc = "\n\nEquipment: " + eqDesc;
        body.text += eqDesc;
    }
    public void GainFocus()
    {
    }
    public void SetSubject(GameObject newSubject)
    {
        subject = newSubject;
        RefreshUI();

        if (subject == null)
            return;

        newSubject.FireEvent(new LookedAtEvent());
    }
    public void SetSide(bool isLeft)
    {
        if (isLeft)
        {
            panelRectTransform.anchorMin = new Vector2(0, 0);
            panelRectTransform.anchorMax = new Vector2(0, 1);
            panelRectTransform.pivot = new Vector2(0, 0);
            panelRectTransform.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            panelRectTransform.anchorMin = new Vector2(1, 0);
            panelRectTransform.anchorMax = new Vector2(1, 1);
            panelRectTransform.pivot = new Vector2(1, 0);
            float offset = Screen.width - Camera.main.pixelWidth;
            panelRectTransform.anchoredPosition = new Vector2(-offset, 0);
        }
    }
}
