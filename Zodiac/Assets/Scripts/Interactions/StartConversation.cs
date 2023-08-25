using System;
using UI;
using UnityEngine;

class StartConversation : IInteraction
{
    public string Name => "Talk to";
    private string _convoStartNode;
    private GameObject _speaker;

    public StartConversation(string startNodeId, GameObject speaker)
    {
        _convoStartNode = startNodeId;
        _speaker = speaker;
    }

    public void Perform()
    {
        _speaker.FireEvent(new SpokenToEvent());
        MenuManager.Instance.ShowConversation(_convoStartNode, _speaker);
    }
}