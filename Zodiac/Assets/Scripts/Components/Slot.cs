using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : ZodiacComponent
{
    [field: SerializeField] public SlotType SlotType { get; set; } // what type of Equippable can be in this slot
    [field: SerializeField] public GameObject Contained { get; set; } // what Equippable is in this slot
    [field: SerializeField] public string SpecialName { get; set; } = ""; // special name: ie, "left hand" or "right hand" or "forelimb" instead of "arm" for nonhuman creatures

    public bool Empty => Contained == null;
    public string GetName()
    {
        if(SpecialName == "")
        {
            return SlotType.ToString();
        }
        return SpecialName;
    }
    public string GetNameWithItem()
    {
        string itemName = "Empty";
        if(Contained != null)
            itemName = Contained.GetComponent<Visual>().DisplayName;

        return $"{GetName()}: {itemName}";
    }
}

public enum SlotType
{
    Hand,
    Missile, // for your ranged weapon
    Head,
    Body,
    Legs
}