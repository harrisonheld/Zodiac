using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class ZodiacInput
{
    private static ZodiacInputMap inputMap;
    public static ZodiacInputMap InputMap { get => inputMap; }

    private static GameObject cursor;
    private static Vector2Int lookCursorPos;

    private enum InputMode
    {
        FreeRoam,
        Look,
        Menu,
    }
    private static InputMode inputMode = InputMode.FreeRoam;

    static ZodiacInput()
    {
        // initialize
        inputMap = new ZodiacInputMap();
        inputMap.Enable();

        cursor = GameObject.Find("Cursor");
    }

    /// <summary>
    /// Returns true when the player has finished their turn.
    /// False if they have not.
    /// </summary>
    /// <param name="_player">The GameObject which is the player.</param>
    /// <returns></returns>
    public static bool DoPlayerInput()
    {
        // player voluntarilly forfeits their turn
        if (inputMap.FreeRoam.ForfeitTurn.triggered)
            return true;
        // out of energy, turn is over
        if (GameManager.Instance.ThePlayer.GetComponent<EnergyHaver>().Energy <= 0)
            return true;

        switch (inputMode)
        {
            case InputMode.FreeRoam:
                DoFreeRoamInput();
                break;

            case InputMode.Look:
                DoLookInput();
                break;

            default:
                break;
        }
        
        return false;
    }

    private static void DoFreeRoamInput()
    {
        // useful stuff
        Vector2Int playerPos = GameManager.Instance.ThePlayer.GetComponent<Position>().Pos;

        // look mode
        if(inputMap.FreeRoam.GoToLookMode.triggered)
        {
            LookMode();
            return;
        }

        // get item
        if (inputMap.FreeRoam.Pickup.triggered)
        {
            /*
            var items = new List<Item>();
            var pickupCandidates = GameManager.EntitiesAt(playerPos);
            foreach (GameObject candidate in pickupCandidates)
            {
                Item item = candidate.GetComponent<Item>();
                if (item != null)
                    items.Add(item);
            }

            foreach (Item item in items)
                GameManager.Pickup(player, item);

            return;
            */
            
            // find all entities with item component
            var items = new List<GameObject>();
            foreach (GameObject obj in GameManager.Instance.EntitiesAt(playerPos))
            {
                if (obj.GetComponent<Item>())
                    items.Add(obj);
            }

            if (items.Count == 1)
                GameManager.Instance.Pickup(GameManager.Instance.ThePlayer, items[0]);
            else if (items.Count > 1)
                PickMenu.Instance.PickMultiple(items, (GameObject item) => GameManager.Instance.Pickup(GameManager.Instance.ThePlayer, item));
            // else if (items.Count == 0), do nothing cuz theres nothing here

            return;
        }

        // open inventory
        if (inputMap.FreeRoam.OpenInventory.triggered)
        {
            var inv = GameManager.Instance.ThePlayer.GetComponent<Inventory>();
            InventoryMenu.Instance.SetInventory(inv);
            MenuManager.Instance.Open(InventoryMenu.Instance);

            return;
        }

        // movement
        Vector2Int move = Vector2Int.RoundToInt(inputMap.FreeRoam.Move.ReadValue<Vector2>());
        if (move != Vector2Int.zero)
        {
            inputMap.FreeRoam.Move.Reset();

            Position playerPosComp = GameManager.Instance.ThePlayer.GetComponent<Position>();
            Vector2Int destPosition = playerPosComp.Pos + move;

            if (GameManager.Instance.isValidMovePosition(destPosition))
            {
                // move
                GameManager.Instance.Move(GameManager.Instance.ThePlayer, playerPosComp.Pos + move);
            }
            else
            {
                // attack
                GameObject target = GameManager.Instance.EntityAt(destPosition);
                GameManager.Instance.BumpAttack(GameManager.Instance.ThePlayer, target);
            }

            return;
        }
    }
    private static void DoLookInput()
    {
        // back to free roam mode
        if(inputMap.Look.Cancel.triggered)
        {
            FreeRoamMode();
            return;
        }

        // move cursor
        Vector2Int move = Vector2Int.RoundToInt(inputMap.FreeRoam.Move.ReadValue<Vector2>());
        if (move != Vector2Int.zero)
        {
            inputMap.FreeRoam.Move.Reset();
            lookCursorPos += move;
            cursor.transform.position = (Vector2)lookCursorPos;

            // no need to check if null, lookmenu will handle that
            GameObject lookingAt = GameManager.Instance.EntityAt(lookCursorPos);
            LookMenu.Instance.SetSubject(lookingAt);

            return;
        }
    }

    public static void FreeRoamMode()
    {
        // hide cursor
        cursor.SetActive(false);
        // hide look menu
        if(MenuManager.Instance.isOpen(LookMenu.Instance))
            MenuManager.Instance.Close(LookMenu.Instance);

        inputMode = InputMode.FreeRoam;
    }
    public static void MenuMode()
    {
        inputMode = InputMode.Menu;
    }
    public static void LookMode()
    {
        // setup cursor
        lookCursorPos = GameManager.Instance.ThePlayer.GetComponent<Position>().Pos;
        cursor.transform.transform.position = (Vector2)lookCursorPos;
        cursor.SetActive(true);

        // show look menu
        LookMenu.Instance.SetSubject(GameManager.Instance.ThePlayer);
        MenuManager.Instance.Open(LookMenu.Instance);

        inputMode = InputMode.Look;
    }
}