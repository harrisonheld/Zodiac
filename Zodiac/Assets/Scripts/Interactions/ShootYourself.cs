using System;
using UnityEngine;

class ShootYourself : IInteraction
{
    public string Name => "shoot yourself in the fucking head";
    public void Perform()
    {
        StatusMenu.Instance.Log("You shoot yourself in the fucking head.");
        GameManager.Instance.BreakEntity(GameManager.Instance.ThePlayer);
    }
}