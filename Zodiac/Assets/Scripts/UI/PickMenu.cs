using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Linq;

namespace UI
{
    class PickMenu : MonoBehaviour, IZodiacMenu
    {
        public Canvas Canvas { get => GetComponent<Canvas>(); }
        public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }
        public GameObject GameObject { get => gameObject; }

        [SerializeField] GameObject optionsContainer;
        [SerializeField] GameObject buttonPrefab;

        private IList _options;
        private Action<object> _action;
        private Func<object, string> _getName;
        private Func<object, bool> _criterion; // objects not meeting the criterion will be greyed out
        private string _prompt = "Pick";
        private bool _closeOnPick = true;
        private bool _removeOnPick = true;

        private int _selectedIdx = 0;

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

                optionButton.GetComponent<Button>().interactable = _criterion(option);

                optionButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _action(option);

                    if (_closeOnPick)
                    {
                        MenuManager.Instance.Close(this);
                        return;
                    }
                    if (_removeOnPick)
                    {
                        _options.Remove(option);
                        // select next option after removal
                        if (_selectedIdx == _options.Count)
                            _selectedIdx--;
                    }
                    if(_options.Count == 0)
                    {
                        MenuManager.Instance.Close(this);
                        return;
                    }

                    RefreshUI();
                });
            }

            optionsContainer.transform.GetChild(_selectedIdx).GetComponent<Selectable>().Select();
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
        /// Bring up the UI to pick an item from a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options">A list of items to choose from.</param>
        /// <param name="action">What to be done with the item once chosen.</param>
        /// <param name="getName">A method to get a name for the obeject. If this is left null, the option's .ToString() method will be used.</param>
        /// <param name="criterion">Options failing the criterion will be greyed out and unselectable. If this is left null, all objects will pass.</param>
        /// <param name="prompt">The title of the menu.</param>
        /// <param name="closeOnPick">If the menu should close once an option is chosen.</param>
        /// <param name="removeOnPick">If options should be removed from the list once chosen.</param>
        /// <returns></returns>
        public void Pick<T>(IList<T> options,
                               Action<T> action,
                               Func<T, string> getName = null,
                               Func<T, bool> criterion = null,
                               string prompt = "Pick",
                               bool closeOnPick = true,
                               bool removeOnPick = true)
        {
            // fill in optional parameters
            if (getName == null)
                getName = obj => obj.ToString();
            if (criterion == null)
                criterion = obj => true;

            // convert to more generic types
            _options = options.Cast<object>().ToList();
            _action = pickable => action((T)pickable);
            _getName = obj => getName((T)obj);
            _criterion = obj => criterion((T)obj);

            _prompt = prompt;
            _closeOnPick = closeOnPick;
            _removeOnPick = removeOnPick;

            _selectedIdx = 0;

            RefreshUI();
        }
    }
}