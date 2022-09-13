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
    /*
     *  WHEN ADDING SOMETHING NEW HERE, REMEMBER TO MODIFY GameManager.cs TO INITIALIZE IT
     */

    public static MenuManager menuManager;

    public static InventoryMenu inventoryMenu;
    public static ItemSubMenu itemSubMenu;
    public static AlertMenu alertMenu;

    public static GameObject cursor;
    public static LookMenu lookMenu;
}
