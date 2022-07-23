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
    static ZodiacInput()
    {
        // initialize
        inputMap = new ZodiacInputMap();
        inputMap.Enable();
    }

    public static IEnumerator DoPlayerInput(GameObject _player)
    {
        player = _player;
        // useful stuff
        Vector2Int playerPos = player.GetComponent<Physical>().Position;

        while (true)
        {
            // player voluntarilly forfeits their turn
            if (inputMap.FreeRoam.ForfeitTurn.triggered)
                yield break;

            // get item
            if (inputMap.FreeRoam.Pickup.triggered)
            {
                var items = new List<Item>();
                foreach(GameObject candidate in GameManager.EntitiesAt(playerPos))
                {
                    Item item = candidate.GetComponent<Item>();
                    if (item != null)
                        items.Add(item);
                }

                foreach(Item item in items)
                    GameManager.Pickup(player, item);
            }

            // open inventory
            if(inputMap.FreeRoam.OpenInventory.triggered)
            {
                var inv = player.GetComponent<Inventory>();
                Common.inventoryMenu.SetInventory(inv);
                Common.menuManager.Open(Common.inventoryMenu);
                    yield return null;
            }

            // movement
            Vector2Int move = Vector2Int.RoundToInt(inputMap.FreeRoam.Move.ReadValue<Vector2>());
            if (move != Vector2Int.zero)
            {
                inputMap.FreeRoam.Move.Reset();

                Physical playerPhys = player.GetComponent<Physical>();
                Vector2Int destPosition = playerPhys.Position + move;

                GameObject target = GameManager.EntityAt(destPosition);
                if (target != null && target.GetComponent<Physical>().Solid)
                {
                    // attack
                    GameManager.Attack(player, target);
                    yield break;
                }
                else
                {
                    // move
                    playerPhys.Position += move;
                    yield break;
                }
            }

            // player didnt give any valid input
            yield return null;
        }
    }

    public static void FreeRoamMode()
    {
        inputMap.FreeRoam.Enable();
    }
    public static void MenuMode()
    {
        inputMap.FreeRoam.Disable();
    }
}