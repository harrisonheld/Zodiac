using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Inventory : ZodiacComponent
{
    public List<Item> Items;
    public List<Slot> Slots;

    public void AddItem(Item item)
    {
        Items.Add(item);
    }
    public bool RemoveItem(Item item)
    {
        return Items.Remove(item);
    }

    public bool HasItems()
    {
        return Items.Count != 0;
    }

    public bool Equip(Equippable equippable)
    {
        // find open slot
        Slot slot = GetOpenSlot(equippable.slotType);
        
        // if no open slot, empty the first slot of the appropriate type and use that
        if (slot == null)
        {
            slot = GetFirstSlot(equippable.slotType);
            UnequipToItems(slot);
        }

        // check if any good slot was found
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

        // if nothing was removed
        if (equippable == null)
            return;

        // put the item in the items
        AddItem(equippable.GetComponent<Item>());
    }

    public void UnequipEverything()
    {
        foreach (Slot slot in Slots)
            UnequipToItems(slot);
    }

    public Slot GetOpenSlot(SlotType type)
    {
        foreach (Slot slot in Slots)
            if (slot.type == type && slot.Empty())
                return slot;

        return null;
    }
    public Slot GetFirstSlot(SlotType type)
    {
        foreach (Slot slot in Slots)
            if (slot.type == type)
                return slot;

        return null;
    }

    public Equippable GetPrimary()
    {
        if (Slots.Count < 1)
            return null;
        if (Slots[0] == null)
            return null;

        return Slots[0].equippable;
    }

    [System.Serializable]
    public class Slot
    {
        public SlotType type;
        public Equippable equippable;

        public bool Empty()
        {
            return equippable == null;
        }
    }
}