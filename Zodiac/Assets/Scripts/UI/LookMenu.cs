using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace UI
{
    class LookMenu : MonoBehaviour
    {
        public Canvas Canvas { get => GetComponent<Canvas>(); }

        private int _lookIdx; // which entity to look at, if there are multiple
        private Vector2Int _lookCursorPos;

        public static LookMenu Instance { get; private set; }
        public bool isOpen { get => Canvas.enabled; }
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
        [SerializeField] private RectTransform panelRectTransform;
        [SerializeField] private GameObject _cursor;

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
                (string str) => {
                    return str.TrimEnd(',');
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
        public GameObject GetSubject()
        {
            return subject;
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
        public Vector2Int GetCursorPos()
        {
            return _lookCursorPos;
        }


        public void HandleInput(ZodiacInputMap inputMap)
        {
            Vector2Int move = Vector2Int.RoundToInt(inputMap.Look.Move.ReadValue<Vector2>());
            int cycle = (int)inputMap.Look.Cycle.ReadValue<float>();
            // these prevent you from holding down the button to move the cursor
            inputMap.Look.Move.Reset();
            inputMap.Look.Cycle.Reset();

            HandleInputInternal(move, cycle);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="move">In which direction to move the cursor.</param>
        /// <param name="cycle">When scrolling through a list of entities, -1 to scroll back, +1 to scroll up. 0 to do nothing.</param>
        private void HandleInputInternal(Vector2Int move, int cycle)
        {
            if (move != Vector2Int.zero)
            {
                _lookIdx = 0;
                _lookCursorPos += move;
                
                _cursor.transform.position = Camera.main.WorldToScreenPoint(new Vector3(_lookCursorPos.x, _lookCursorPos.y, 0f));

                GameObject lookingAt = GameManager.Instance.EntitiesAt(_lookCursorPos).FirstOrDefault();
                SetSubject(lookingAt);

                bool isLeft = _lookCursorPos.x > (WorldGen.World.GetCurrentZoneWidth / 2);
                SetSide(isLeft);
            }
            else if (cycle != 0)
            {
                List<GameObject> atPos = GameManager.Instance.EntitiesAt(_lookCursorPos);
                if (atPos.Count == 0)
                    return;

                _lookIdx += cycle;
                if (_lookIdx < 0)
                    _lookIdx = atPos.Count - 1;
                else if (_lookIdx >= atPos.Count)
                    _lookIdx = 0;

                SetSubject(atPos[_lookIdx]);
            }
        }
        public void Show(Vector2Int lookCursorPos)
        {
            Canvas.enabled = true;

            SetSubject(GameManager.Instance.EntityAt(lookCursorPos));
            _lookCursorPos = lookCursorPos;

            _cursor.transform.position = Camera.main.WorldToScreenPoint(new Vector3(_lookCursorPos.x, _lookCursorPos.y, 0f));
            _cursor.transform.localScale = Vector3.one * (10f / Camera.main.orthographicSize);

            bool isLeft = _lookCursorPos.x > (WorldGen.World.GetCurrentZoneWidth / 2);
            SetSide(isLeft);

        }
        public void Show(GameObject subject)
        {
            Canvas.enabled = true;

            SetSubject(subject);
            _lookCursorPos = subject.GetComponent<Position>().Pos;

            _cursor.transform.position = Camera.main.WorldToScreenPoint(new Vector3(_lookCursorPos.x, _lookCursorPos.y, 0f));
            _cursor.transform.localScale = Vector3.one * (10f / Camera.main.orthographicSize);

            bool isLeft = _lookCursorPos.x > (WorldGen.World.GetCurrentZoneWidth / 2);
            SetSide(isLeft);

        }
        public void Hide()
        {
            Canvas.enabled = false;
        }

    }
}
