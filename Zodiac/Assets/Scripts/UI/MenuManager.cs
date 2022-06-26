using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    private static Stack<IZodiacMenu> menus = new Stack<IZodiacMenu> ();

    public void Update()
    {
        if (menus.Count == 0)
            return;

        // closing of menus
        if(ZodiacInput.InputMap.UI.Cancel.triggered)
        {
            CloseMenu(menus.Peek());
        }
    }

    public void OpenMenu(IZodiacMenu menu)
    {
        if(menus.Count == 0)
            ZodiacInput.MenuMode();
        else // menus.Count > 0 
        {
            // disable interatcion on previous menu
            menus.Peek().CanvasGroup.interactable = false;
        }

        menu.Canvas.enabled = true;
        menu.CanvasGroup.interactable = true;

        menu.RefreshUI();
        menu.GainFocus();

        menus.Push(menu);
    }
    public void CloseMenu(IZodiacMenu menu)
    {
        Debug.Log("Closing menu.");

        Debug.Assert(menus.Count > 0);
        Debug.Assert(menu == menus.Peek());

        IZodiacMenu toClose = menus.Pop();
        toClose.Canvas.enabled = false;
        toClose.CanvasGroup.interactable = false;

        if (menus.Count == 0)
            ZodiacInput.FreeRoamMode();
        else // next menu gains focus
        {
            menus.Peek().CanvasGroup.interactable = true;
            menus.Peek().GainFocus();
        }
    }
}