using System;
using UnityEngine;

class FreezeTimeForWielder : IInteraction
{
    public string Name => "Congeal Time";
    public void Perform()
    {
        // add ten thousand energy
        GameManager.Instance.ThePlayer.GetComponent<EnergyHaver>().Energy += 10000;
        LogMenu.Instance.Log("The flow of time is clotted as ten seconds coagulate.");
    }
}