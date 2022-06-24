using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI text;
    
    public void SetItem(Item item)
    {
        Visual vis = item.gameObject.GetComponent<Visual>();
        if (vis == null)
            return;

        // set name
        text.text = vis.Name;
        if (item.Count > 1)
            text.text += " " + item.Count;

        // set icon
        icon.sprite = vis.Sprite;
    }
}
