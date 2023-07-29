using System;
using UI;
using UnityEngine;

class StartConversation : IInteraction
{
    public string Name => "Talk to";
    private string _convoStartNode;

    public StartConversation(string startNodeId)
    {
        _convoStartNode = startNodeId;
    }

    public void Perform()
    {
        MenuManager.Instance.ShowConversation(_convoStartNode);
    }
}