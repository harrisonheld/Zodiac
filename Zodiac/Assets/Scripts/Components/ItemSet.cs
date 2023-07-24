using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Entities having this component will have the item set given to them and then this component removed
public class ItemSet : ZodiacComponent
{
    [field: SerializeField] public string ItemSetName { get; set; }
}