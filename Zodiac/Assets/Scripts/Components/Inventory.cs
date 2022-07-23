using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> Items;
    public List<Slot> Slots;

    public void AddItem(Item item)
    {
        // get the gameobject
        GameObject go = item.gameObject;
        // remove the Physical component, if it has one
        Physical phys = go.GetComponent<Physical>();
        Destroy(phys);

        item.ContainingInventory = this;

        Items.Add(item);
    }
    public bool RemoveItem(Item item)
    {
        item.ContainingInventory = null;
        return Items.Remove(item);
    }

    public bool Equip(Equippable equippable)
    {
        Slot slot = GetOpenSlot(equippable.slotType);
        // check if possible to equip
        if (slot == null)
            return false;

        // remove it from inventory if possible
        RemoveItem(equippable.GetComponent<Item>());

        // equip it
        slot.equippable = equippable;
        return true; // success
    }
    public void UnequipToItems(Slot slot)
    {
        // remove the equippable
        Equippable equippable = slot.equippable;
        slot.equippable = null;

        // put the item in the items
        AddItem(equippable.GetComponent<Item>());
    }

    public Slot GetOpenSlot(SlotType type)
    {
        foreach (Slot slot in Slots)
            if (slot.type == type)
                return slot;

        return null;
    }

    [System.Serializable]
    public class Slot
    {
        public SlotType type;
        public Equippable equippable;
    }
}