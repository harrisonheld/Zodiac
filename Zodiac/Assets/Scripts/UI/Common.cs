using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class holds common elements that other scripts might want to use.
/// </summary>
public static class Common
{
    public static MenuManager menuManager;

    // when adding a new menu here, remember to edit the start function in GameManager.cs to intialize it
    public static InventoryMenu inventoryMenu;
    public static ItemSubMenu itemSubMenu;
    public static AlertMenu alertMenu;
}
