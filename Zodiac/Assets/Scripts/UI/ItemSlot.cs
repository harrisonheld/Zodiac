using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI text;

    private GameObject item;
    
    public void SetItem(GameObject _item)
    {
        item = _item;

        Visual vis = item.gameObject.GetComponent<Visual>();
        if (vis == null)
            return;

        // set name
        text.text = vis.DisplayName;
        if (item.GetComponent<Item>().Count > 1)
            text.text += " x" + item.GetComponent<Item>().Count;

        // set icon
        icon.sprite = vis.GetUnitySprite();
    }

    public void ItemSelected()
    {
        ItemSubMenu.Instance.SetItem(item);
        MenuManager.Instance.Open(ItemSubMenu.Instance);
    }
}
