using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Conversation : ZodiacComponent
{
    [field: SerializeField] public string ConversationId { get; set; }

    public override List<IInteraction> GetInteractions() {
        return new List<IInteraction>() {
            new StartConversation(ConversationId)
        };
    }
}