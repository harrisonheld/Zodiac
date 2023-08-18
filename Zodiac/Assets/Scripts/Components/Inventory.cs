using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

[DisallowMultipleComponent]
public class Inventory : ZodiacComponent
{
    [field: SerializeField]  public List<GameObject> Items { get; set; } = new();



    public override void Serialize(BinaryWriter writer)
    {
        WriteEntityList(writer, Items);
    }
    public override void Deserialize(BinaryReader reader, Dictionary<int, GameObject> idToEntity = null)
    {
        Items = ReadEntityList(reader, idToEntity);
    }

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

        return Equip(equippable, slot);
    }
    public bool Equip(GameObject equippable, Slot slot)
    {
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
        foreach (Slot slot in gameObject.GetComponents<Slot>())
            UnequipToItems(slot);
    }

    public Slot GetOpenSlot(SlotType type)
    {
        foreach (Slot slot in gameObject.GetComponents<Slot>())
            if (slot.SlotType == type && slot.Empty)
                return slot;

        return null;
    }
    public Slot GetFirstSlot(SlotType type)
    {
        foreach (Slot slot in gameObject.GetComponents<Slot>())
            if (slot.SlotType == type)
                return slot;

        return null;
    }
}