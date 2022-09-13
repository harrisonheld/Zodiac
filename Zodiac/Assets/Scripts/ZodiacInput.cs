using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class ZodiacInput
{
    private static ZodiacInputMap inputMap;
    public static ZodiacInputMap InputMap { get => inputMap; }

    // the current player gameobject who is doing input
    private static GameObject player;
    public static GameObject Player { get => player; }

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
    }

    public static IEnumerator DoPlayerInput(GameObject _player)
    {
        player = _player;

        while (true)
        {
            // player voluntarilly forfeits their turn
            if (inputMap.FreeRoam.ForfeitTurn.triggered)
                yield break;
            // out of energy, turn is over
            if (player.GetComponent<EnergyHaver>().Energy <= 0)
                yield break;
            // player is no longer in control of this character
            if (player.GetComponent<Brain>().Ai != AiType.PlayerControlled)
                yield break;

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

            // player didnt give any valid input, so try again
            yield return null;
        }
    }

    private static void DoFreeRoamInput()
    {
        // useful stuff
        Vector2Int playerPos = player.GetComponent<Position>().Pos;

        // look mode
        if(inputMap.FreeRoam.GoToLookMode.triggered)
        {
            LookMode();
            return;
        }

        // get item
        if (inputMap.FreeRoam.Pickup.triggered)
        {
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
        }

        // open inventory
        if (inputMap.FreeRoam.OpenInventory.triggered)
        {
            Debug.Log("Open Inventory button pressed.");
            var inv = player.GetComponent<Inventory>();
            Common.inventoryMenu.SetInventory(inv);
            Common.menuManager.Open(Common.inventoryMenu);

            return;
        }

        // movement
        Vector2Int move = Vector2Int.RoundToInt(inputMap.FreeRoam.Move.ReadValue<Vector2>());
        if (move != Vector2Int.zero)
        {
            inputMap.FreeRoam.Move.Reset();

            Position playerPosComp = player.GetComponent<Position>();
            Vector2Int destPosition = playerPosComp.Pos + move;

            if (GameManager.isValidMovePosition(destPosition))
            {
                // move
                GameManager.Move(player, playerPosComp.Pos + move);
            }
            else
            {
                // attack
                GameObject target = GameManager.EntityAt(destPosition);
                GameManager.BumpAttack(player, target);
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
            Common.cursor.transform.position = (Vector2)lookCursorPos;

            // no need to check if null, lookmenu will handle that
            GameObject lookingAt = GameManager.EntityAt(lookCursorPos);
            Common.lookMenu.SetSubject(lookingAt);

            return;
        }
    }

    public static void FreeRoamMode()
    {
        // disable look mode stuff
        Common.cursor.SetActive(false);
        Common.lookMenu.Canvas.enabled = false;

        inputMode = InputMode.FreeRoam;
    }
    public static void MenuMode()
    {
        inputMode = InputMode.Menu;
    }
    public static void LookMode()
    {
        // setup cursor
        lookCursorPos = player.GetComponent<Position>().Pos;
        Common.cursor.transform.transform.position = (Vector2)lookCursorPos;
        Common.cursor.SetActive(true);

        // show look menu
        Common.lookMenu.Canvas.enabled = true;

        inputMode = InputMode.Look;
    }
}