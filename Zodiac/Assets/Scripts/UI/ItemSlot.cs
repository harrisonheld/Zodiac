using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI text;

    private Item item;
    
    public void SetItem(Item _item)
    {
        item = _item;

        Visual vis = item.gameObject.GetComponent<Visual>();
        if (vis == null)
            return;

        // set name
        text.text = vis.DisplayName;
        if (item.Count > 1)
            text.text += " x" + item.Count;

        // set icon
        icon.sprite = vis.GetUnitySprite();
    }

    public void ItemSelected()
    {
        Common.itemSubMenu.SetItem(item);
        Common.menuManager.Open(Common.itemSubMenu);
    }
}
