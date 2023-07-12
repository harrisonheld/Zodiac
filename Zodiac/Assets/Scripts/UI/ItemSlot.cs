using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using static UnityEditor.Progress;

namespace UI
{
    class ItemSlot : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI text;

        private GameObject _item;

        public void SetItem(GameObject item)
        {
            _item = item;

            Visual vis = _item.gameObject.GetComponent<Visual>();
            if (vis == null)
                return;

            // set name
            text.text = vis.DisplayName;
            if (_item.GetComponent<Item>().Count > 1)
                text.text += " x" + _item.GetComponent<Item>().Count;

            // set icon
            icon.sprite = vis.GetUnitySprite();
        }

        public void ItemSelected()
        {
            MenuManager.Instance.ShowItemSubMenu(_item);
        }
    }
}
