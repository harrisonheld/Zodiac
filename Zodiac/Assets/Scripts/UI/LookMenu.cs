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

    private static GameObject cursor;
    private static int lookIdx; // which entity to look at, if there are multiple
    private static Vector2Int _lookCursorPos;

    [SerializeField] private RectTransform panelRectTransform;

    public static LookMenu Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(this.gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;

            cursor = GameObject.Find("Cursor");
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
        if (health != null)
            healthText.text = health.GetHealthString();
        else
            healthText.text = "";

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

    public void ShowCursor(Vector2Int pos)
    {
        cursor.transform.position = (Vector2)pos;
        cursor.SetActive(true);
    }
    public void HideCursor()
    {
        cursor.SetActive(false);
    }

    /// <summary>
    /// a
    /// </summary>
    /// <param name="move">In which direction to move the cursor.</param>
    /// <param name="cycle">When scrolling through a list of entities, -1 to scroll back, +1 to scroll up. 0 to do nothing.</param>
    public void HandleInput(Vector2Int move, int cycle)
    {
        if(move != Vector2Int.zero)
        {
            lookIdx = 0;
            _lookCursorPos += move;

            cursor.transform.position = (Vector2)_lookCursorPos;

            GameObject lookingAt = GameManager.Instance.EntitiesAt(_lookCursorPos).FirstOrDefault();
            SetSubject(lookingAt);

            bool isLeft = _lookCursorPos.x > (WorldGen.World.SCREEN_WIDTH / 2);
            SetSide(isLeft);
        }
        else if (cycle != 0)
        {
            List<GameObject> atPos = GameManager.Instance.EntitiesAt(_lookCursorPos);
            if (atPos.Count == 0)
                return;

            lookIdx += cycle;
            if (lookIdx < 0)
                lookIdx = atPos.Count - 1;
            else if (lookIdx >= atPos.Count)
                lookIdx = 0;

            SetSubject(atPos[lookIdx]);
        }
    }
    public void Show(Vector2Int lookCursorPos)
    {
        _lookCursorPos = lookCursorPos;

        cursor.SetActive(true);
        cursor.transform.position = (Vector2)lookCursorPos;
        bool isLeft = lookCursorPos.x > (WorldGen.World.SCREEN_WIDTH / 2);

        SetSide(isLeft);
        SetSubject(GameManager.Instance.EntityAt(lookCursorPos));

        MenuManager.Instance.Open(this);
    }
    public void HideLookMenu()
    {
        MenuManager.Instance.Close(this);
    }

}
