using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Conversation : ZodiacComponent
{
    [field: SerializeField] public string ConversationId { get; set; }
}