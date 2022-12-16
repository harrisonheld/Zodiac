using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Inventory : ZodiacComponent
{
    [field: SerializeField]  public List<Item> Items { get; set; } = new();
    [field: SerializeField]  public List<Slot> Slots { get; set; } = new();

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
        // see if creature even has a good slot
        if (GetFirstSlot(equippable.SlotType) == null)
            return false;
        
        // find open slot
        Slot slot = GetOpenSlot(equippable.SlotType);
        
        // if no open slot, empty the first slot of the appropriate type and use that
        if (slot == null)
        {
            slot = GetFirstSlot(equippable.SlotType);
            UnequipToItems(slot);
        }

        // check if any good slot was found
        if (slot == null)
            return false;

        // remove it from inventory if possible
        RemoveItem(equippable.GetComponent<Item>());

        // equip it
        slot.Equippable = equippable;
        return true; // success
    }
    public void UnequipToItems(Slot slot)
    {
        // remove the equippable
        Equippable equippable = slot.Equippable;
        slot.Equippable = null;

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

    public Equippable GetPrimary()
    {
        if (Slots.Count == 0)
            return null;
        if (Slots[0] == null)
            return null;

        return Slots[0].Equippable;
    }

    [System.Serializable]
    public class Slot
    {
        [field: SerializeField] public SlotType Type { get; set; }
        [field: SerializeField] public Equippable Equippable { get; set; }

        public bool Empty()
        {
            return Equippable == null;
        }
    }
}