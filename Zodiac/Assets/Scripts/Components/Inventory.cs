using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[DisallowMultipleComponent]
public class Inventory : ZodiacComponent
{
    [field: SerializeField]  public List<GameObject> Items { get; set; } = new();
    [field: SerializeField]  public List<Slot> Slots { get; set; } = new();

    public void AddItem(GameObject item)
    {
        Items.Add(item);
    }
    public bool RemoveItem(GameObject item)
    {
        return Items.Remove(item);
    }

    public bool HasItems()
    {
        return Items.Count != 0;
    }

    public bool Equip(GameObject equippable)
    {
        SlotType slotType = equippable.GetComponent<Equippable>().SlotType;
        // see if creature even has a good slot
        if (GetFirstSlot(slotType) == null)
            return false;
        
        // find open slot
        Slot slot = GetOpenSlot(slotType);
        
        // if no open slot, empty the first slot of the appropriate type and use that
        if (slot == null)
        {
            slot = GetFirstSlot(slotType);
            UnequipToItems(slot);
        }

        // check if any good slot was found
        if (slot == null)
            return false;

        // remove it from inventory if possible
        RemoveItem(equippable);

        // equip it
        slot.Contained = equippable;
        return true; // success
    }
    public void UnequipToItems(Slot slot)
    {
        // remove the equippable
        GameObject equippable = slot.Contained;
        slot.Contained = null;

        // if nothing was removed
        if (equippable == null)
            return;

        // put the item in the items
        AddItem(equippable);
    }

    public void UnequipEverything()
    {
        foreach (Slot slot in Slots)
            UnequipToItems(slot);
    }

    public Slot GetOpenSlot(SlotType type)
    {
        foreach (Slot slot in Slots)
            if (slot.Type == type && slot.Empty())
                return slot;

        return null;
    }
    public Slot GetFirstSlot(SlotType type)
    {
        foreach (Slot slot in Slots)
            if (slot.Type == type)
                return slot;

        return null;
    }

    public GameObject GetPrimary()
    {
        if (Slots.Count == 0)
            return null;
        if (Slots[0] == null)
            return null;

        return Slots[0].Contained;
    }

    [System.Serializable]
    public class Slot
    {
        [field: SerializeField] public SlotType Type { get; set; }
        [field: SerializeField] public GameObject Contained { get; set; }

        public bool Empty()
        {
            return Contained == null;
        }
    }
}