using System;
using UnityEngine;

public interface IInteraction
{
    public string Name { get; }
    public void Perform();
}