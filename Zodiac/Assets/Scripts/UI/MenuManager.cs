using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    // singleton
    public static MenuManager Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(this.gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    // list of menus, in order of precedence. The menu at the end is the topmost menu.
    // i would use a stack but sometimes you want to close a menu that isnt the top most.
    private static List<IZodiacMenu> menus = new List<IZodiacMenu> ();

    public void Update()
    {
        if (menus.Count == 0)
            return;

        // closing of menus
        if(ZodiacInput.InputMap.UI.Cancel.triggered)
            CloseTopMenu();
    }

    public IZodiacMenu TopMenu()
    {
        return menus.Last();
    }
    public void CloseTopMenu()
    {
        Close(TopMenu());
    }
    public void CloseAll()
    {
        while (menus.Count > 0)
            Close(TopMenu());
    }

    public void Open(IZodiacMenu menu)
    {
        if(menus.Count == 0)
            ZodiacInput.MenuMode();
        else // menus.Count > 0 
        {
            // disable interatcion on previous menu
            TopMenu().CanvasGroup.interactable = false;
        }

        menu.Canvas.enabled = true;
        menu.CanvasGroup.interactable = true;

        menu.RefreshUI();
        menu.GainFocus();

        menus.Add(menu);
    }
    public void Close(IZodiacMenu toClose)
    {
        menus.Remove(toClose);
        toClose.Canvas.enabled = false;
        toClose.CanvasGroup.interactable = false;

        if (menus.Count == 0)
            ZodiacInput.FreeRoamMode();
        else // next menu gains focus
        {
            TopMenu().CanvasGroup.interactable = true;
            TopMenu().GainFocus();
        }
    }

    public bool isOpen(IZodiacMenu toCheck)
    {
        return toCheck.Canvas.enabled;
    }
}