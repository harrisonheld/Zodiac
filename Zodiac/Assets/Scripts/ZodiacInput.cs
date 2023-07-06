using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public static class ZodiacInput
{
    private static ZodiacInputMap inputMap;
    public static ZodiacInputMap InputMap { get => inputMap; }

    private static GameObject cursor;
    private static int lookIdx; // which entity to look at, if there are multiple
    private static Vector2Int lookCursorPos;

    private enum InputMode
    {
        FreeRoam,
        Look,
        Interact,
        Menu,
        AbilityTargetSelection,
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
        // out of energy, turn is over
        if (GameManager.Instance.ThePlayer.GetComponent<EnergyHaver>().Energy <= 0)
            return true;

        switch (inputMode)
        {
            case InputMode.FreeRoam:
                // player voluntarilly forfeits their turn
                if (inputMap.FreeRoam.ForfeitTurn.triggered)
                    return true;
                DoFreeRoamInput();
                break;

            case InputMode.Look:
                DoLookInput();
                break;

            case InputMode.Interact:
                DoInteractInput();
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

        // interact mode
        if(inputMap.FreeRoam.GoToInteractMode.triggered)
        {
            InteractMode();
            return;
        }

        // get item
        if (inputMap.FreeRoam.Pickup.triggered)
        {
            // find all entities with item component
            var items = new List<GameObject>();
            foreach (GameObject obj in GameManager.Instance.EntitiesAt(playerPos))
            {
                if (obj.GetComponent<Item>())
                    items.Add(obj);
            }

            if(items.Count == 0)
                StatusMenu.Instance.Log("There's nothing here to pick up.");
            else if (items.Count == 1)
                GameManager.Instance.Pickup(GameManager.Instance.ThePlayer, items[0]);
            else if (items.Count > 1)
                PickMenu.Instance.PickMultiple(items, (GameObject item) => GameManager.Instance.Pickup(GameManager.Instance.ThePlayer, item));

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
        Vector2Int move = Vector2Int.RoundToInt(inputMap.Look.Move.ReadValue<Vector2>());
        int cycle = (int)InputMap.Look.Cycle.ReadValue<float>();
        if (move != Vector2Int.zero)
        {
            inputMap.Look.Move.Reset();
            
            lookIdx = 0;

            lookCursorPos += move;
            cursor.transform.position = (Vector2)lookCursorPos;

            // no need to check if null, lookmenu will handle that
            GameObject lookingAt = GameManager.Instance.EntitiesAt(lookCursorPos).FirstOrDefault();
            LookMenu.Instance.SetSubject(lookingAt);

            bool isLeft = lookCursorPos.x > (WorldGen.World.SCREEN_WIDTH / 2);
            LookMenu.Instance.SetSide(isLeft);

            return;
        }
        else if(cycle != 0)
        {
            List<GameObject> atPos = GameManager.Instance.EntitiesAt(lookCursorPos);
            if (atPos.Count == 0)
                return;
            
            lookIdx += cycle;
            if (lookIdx < 0)
                lookIdx = atPos.Count - 1;
            else if (lookIdx >= atPos.Count)
                lookIdx = 0;

            LookMenu.Instance.SetSubject(atPos[lookIdx]);
        }
    }
    private static void DoInteractInput()
    {
        // back to free roam mode
        if (inputMap.Interact.Cancel.triggered)
        {
            FreeRoamMode();
            return;

        }
        Vector2Int dir = Vector2Int.RoundToInt(inputMap.Interact.PickDir.ReadValue<Vector2>());
        if (dir != Vector2Int.zero)
        {
            inputMap.Interact.PickDir.Reset();

            Vector2Int selectedPos = GameManager.Instance.ThePlayer.GetComponent<Position>().Pos + dir;
            GameObject selected = GameManager.Instance.EntityAt(selectedPos);
            if(selected == null)
            {
                StatusMenu.Instance.Log("There is nothing to interact with there.");
                return;
            }

            List<IInteraction> interactions = selected.GetInteractions();
            if(interactions.Count == 0)
            {
                StatusMenu.Instance.Log("There is no way to interact with that.");
                return;
            }

            interactions[0].Perform();
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
    public static void LookMode()
    {
        // setup cursor
        lookCursorPos = GameManager.Instance.ThePlayer.GetComponent<Position>().Pos;
        cursor.transform.transform.position = (Vector2)lookCursorPos;
        cursor.SetActive(true);

        // show look menu
        LookMenu.Instance.SetSubject(GameManager.Instance.ThePlayer);
        MenuManager.Instance.Open(LookMenu.Instance);

        // put it on correct side
        bool isLeft = lookCursorPos.x > (WorldGen.World.SCREEN_WIDTH / 2);
        LookMenu.Instance.SetSide(isLeft);

        inputMode = InputMode.Look;
    }
    public static void InteractMode()
    {
        inputMode = InputMode.Interact;
    }
    public static void MenuMode()
    {
        inputMode = InputMode.Menu;
    }
}