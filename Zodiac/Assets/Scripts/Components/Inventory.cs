using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> Contents;

    public void AddItem(Item item)
    {
        // get the gameobject
        GameObject go = item.gameObject;
        // remove the Physical component, if it has one
        Physical phys = go.GetComponent<Physical>();
        Destroy(phys);

        Contents.Add(item);
    }
    public bool RemoveItem(Item item)
    {
        return Contents.Remove(item);
    }
}