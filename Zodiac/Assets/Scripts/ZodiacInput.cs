using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public static class ZodiacInput
{
    private static ZodiacInputMap inputMap;
    public static ZodiacInputMap InputMap { get => inputMap; }

    private static AbilityBase abilityToUse; // what ability the player is going to use

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
    }

    /// <summary>
    /// Returns true when the player has finished their turn.
    /// False if they have not.
    /// </summary>
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

            case InputMode.Menu:
                DoMenuInput();
                break;

            case InputMode.AbilityTargetSelection:
                DoAbilityTargetSelectionInput();
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

            if (items.Count == 0)
            {
                StatusMenu.Instance.Log("There's nothing here to pick up.");
            }
            else if (items.Count == 1)
            {
                GameManager.Instance.Pickup(GameManager.Instance.ThePlayer, items[0]);
            }
            else if (items.Count > 1)
            {
                PickMenu.Instance.PickOne(
                    options: items,
                    getName: item => item.GetComponent<Visual>().DisplayName, 
                    action: item => GameManager.Instance.Pickup(GameManager.Instance.ThePlayer, item),
                    prompt: "Pick up what?", 
                    closeOnPick: false 
                );

            }
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

        // open abilities menu
        if (inputMap.FreeRoam.OpenAbilities.triggered)
        {
            var abilities = GameManager.Instance.ThePlayer.GetComponents<AbilityBase>();

            PickMenu.Instance.PickOne(
                abilities, 
                ability => ability.GetShortName(),
                ability => AbilityTargetSelectionMode(ability),
                prompt: "Use which ability?"
            );
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

        // look menu input
        Vector2Int move = Vector2Int.RoundToInt(inputMap.Look.Move.ReadValue<Vector2>());
        int cycle = (int)InputMap.Look.Cycle.ReadValue<float>();
        LookMenu.Instance.HandleInput(move, cycle);

        // these prevent you from holding down the button to move the cursor
        inputMap.Look.Move.Reset();
        inputMap.Look.Cycle.Reset();
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
    private static void DoMenuInput()
    {
        if(!MenuManager.Instance.AnyMenusOpen())
        {
            inputMode = InputMode.FreeRoam;
        }
    }
    private static void DoAbilityTargetSelectionInput()
    {
        // cancel
        if(inputMap.UI.Cancel.triggered)
        {
            FreeRoamMode();
            return;
        }

        // target picking
        bool selectionDone = abilityToUse.TargetingMechanism.HandleInput(inputMap);

        // ability activation
        if (selectionDone)
        {
            abilityToUse.Activate();
            FreeRoamMode();
            return;
        }
    }

    public static void FreeRoamMode()
    {
        // hide look menu
        LookMenu.Instance.HideLookMenu();

        inputMode = InputMode.FreeRoam;
    }
    public static void LookMode()
    {
        LookMenu.Instance.Show(GameManager.Instance.ThePlayer.GetComponent<Position>().Pos);
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
    public static void AbilityTargetSelectionMode(AbilityBase ability)
    {
        abilityToUse = ability;
        inputMode = InputMode.AbilityTargetSelection;
    }
}