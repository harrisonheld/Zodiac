using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UI;
using Raws;

public class GameManager : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private int turn = 0;
    [SerializeField] private int screenX = 2;
    [SerializeField] private int screenY = 15;

    private GameSave gameSave;

    // a list of Entities that have Position components - IE, ones that exist on the map - not in inventories or things like that
    public List<GameObject> Entities = new List<GameObject>();
    [SerializeField] public GameObject ThePlayer;

    private List<ISystem> Systems = new List<ISystem>();

    // singleton pattern
    public static GameManager Instance { get; private set; }

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

    public void Start()
    {
        CreateNewGameSave();

        RegisterSystem<EnergySystem>();
        RegisterSystem<BrainSystem>();
        RegisterSystem<CooldownSystem>();

        WorldGen.World.SetWorldSeed(gameSave.WorldSeed);
        WorldGen.World.GenerateZone(screenX, screenY);

        ThePlayer = Blueprints.FromBlueprint("You", new Vector2Int(9, 5));

        Blueprints.FromBlueprint("EnthralledAlchemist", new Vector2Int(5, 5));
        Blueprints.FromBlueprint("Staccato", new Vector2Int(4, 5));
        Blueprints.FromBlueprint("FrogWarden", new Vector2Int(3, 4));
        Blueprints.FromBlueprint("Judas", new Vector2Int(3, 4));
    }

    public void Update()
    {
        // death
        if(ThePlayer == null)
        {
            // tell the player this
            if (!MenuManager.Instance.isOpen(AlertMenu.Instance))
            {
                MenuManager.Instance.CloseAll();
                MenuManager.Instance.ShowAlert("Unfortunately, you have died or otherwise ceased to exist.");
            }
            return;
        }

        bool inputDone = ZodiacInput.DoPlayerInput();
        if (inputDone)
        {
            foreach (ISystem system in Systems) {
                system.Tick();
            }

            turn++;

            Position playerPos = ThePlayer.GetComponent<Position>();
            double playerU = playerPos.X / (double)(WorldGen.World.GetCurrentZoneWidth -1);
            double playerV = playerPos.Y / (double)(WorldGen.World.GetCurrentZoneHeight - 1);

            bool leftScreen = false;
            if (playerV > 1.0)
            {
                playerV = 0.0;
                gameSave.SaveScreen(screenX, screenY);
                screenY++;
                leftScreen = true;
            }
            else if(playerV < 0.0)
            {
                playerV = 1.0;
                gameSave.SaveScreen(screenX, screenY);
                screenY--;
                leftScreen = true;
            }
            if (playerU > 1.0)
            {
                playerU = 0.0;
                gameSave.SaveScreen(screenX, screenY);
                screenX++;
                leftScreen = true;
            }
            else if (playerU < 0.0)
            {
                playerU = 1.0;
                gameSave.SaveScreen(screenX, screenY);
                screenX--;
                leftScreen = true;
            }

            if (leftScreen)
            {
                foreach(GameObject entity in Entities)
                {
                    if (entity != ThePlayer)
                        DestroyImmediate(entity);
                }
                Entities.Clear();
                Entities.Add(ThePlayer);

                if (gameSave.ScreenSaved(screenX, screenY))
                    Entities.AddRange(gameSave.LoadScreen(screenX, screenY));
                else
                    WorldGen.World.GenerateZone(screenX, screenY);

                playerPos.X = (int)(playerU * (WorldGen.World.GetCurrentZoneWidth - 1));
                playerPos.Y = (int)(playerV * (WorldGen.World.GetCurrentZoneHeight - 1));
            }
        }
    }
    public int GetTurn()
    {
        return turn;
    }

    private void RegisterSystem<T>() where T : ISystem, new() {
        ISystem system = new T();
        Systems.Add(system);
    }

    /// <summary>
    /// Get the entity at the specified position.
    /// </summary>
    public GameObject EntityAt(Vector2Int pos)
    {
        foreach(GameObject entity in Entities)
        {
            if (entity.GetComponent<Position>().Pos == pos)
                return entity;
        }

        return null;
    }
    public List<GameObject> EntitiesAt(Vector2Int pos)
    {
        var result = new List<GameObject>();

        foreach (GameObject entity in Entities)
        {
            if (entity.GetComponent<Position>().Pos == pos)
                result.Add(entity);
        }

        return result;
    }

    public GameObject SolidEntityAt(Vector2Int pos)
    {
        foreach (GameObject entity in EntitiesAt(pos))
        {
            if (entity.GetComponent<PhysicalAttributes>().Solid)
                return entity;
        }

        return null;
    }
    public void BreakEntity(GameObject toDestroy)
    {
        DropAll(toDestroy);

        // tell user
        string deathMessage = $"The {toDestroy.GetComponent<Visual>().DisplayName} is destroyed.";
        if (toDestroy.GetComponent<Living>() != null)
            deathMessage = $"The {toDestroy.GetComponent<Visual>().DisplayName} dies.";
        MenuManager.Instance.Log(deathMessage);

        // bye bye!
        Entities.Remove(toDestroy);
        GameObject.Destroy(toDestroy);
    }


    public bool isValidMovePosition(Vector2Int toCheck)
    {
        // position is valid if nothing solid is here
        return SolidEntityAt(toCheck) == null;
    }

    /// <summary>
    /// Move to the specified position with no checks. Energy is deducted.
    /// </summary>
    public void Move(GameObject toMove, Vector2Int destination)
    {
        toMove.GetComponent<EnergyHaver>().Energy -= Constants.COST_MOVE;
        toMove.GetComponent<Position>().SmoothMove(destination);
    }
    /// <summary>
    /// Attempts to move to this destination, checking if the destination position is valid.
    /// </summary>
    /// <returns><see langword="true"/> if the move was successful, <see langword="false"/> otherwise.</returns>
    public bool AttemptMove(GameObject toMove, Vector2Int destination)
    {
        if (isValidMovePosition(destination))
        {
            Move(toMove, destination);
            return true;
        }

        return false;
    }

    public void BumpAttack(GameObject attacker, GameObject target)
    {
        var targetHealth = target.GetComponent<Health>();
        if (targetHealth == null)
        {
            Debug.LogError("Failed to attack as target has no health component.");
            return;
        }

        // animate a little bump
        attacker.GetComponent<Position>().VisualBump(target.GetComponent<Position>().Pos);

        // in the case of no weapon, use these default values
        string attackDamage = "1d2";
        int attackCost = 1000; // cost of the attack in energy

        // get the first melee weapon we have equipped
        GameObject attackerPrimary = attacker.GetComponents<Slot>()
                                             .FirstOrDefault(s => s.Contained?.GetComponent<MeleeWeapon>() != null)?
                                             .Contained;
        if (attackerPrimary != null)
        {
            MeleeWeapon weapon = attackerPrimary.GetComponent<MeleeWeapon>();
            if(weapon != null)
            {
                attackDamage = weapon.Damage;
                attackCost = weapon.AttackCost;
            }
        }

        attacker.GetComponent<EnergyHaver>().Energy -= attackCost;

        Stats attackerStats = attacker.GetEffectiveStats();
        Stats targetStats   =   target.GetEffectiveStats();

        int toHit = Dice.Roll("1d20") + (attackerStats["Prowess"] - 10) / 2;
        int dodge = (targetStats["Dexterity"] - 10) / 2;
        if(toHit < dodge)
        {
            // tell user about the miss
            string missMessage = $"The {attacker.GetComponent<Visual>().DisplayName} misses the {target.GetComponent<Visual>().DisplayName}.";
            if (attacker == ThePlayer)
            {
                missMessage = $"You miss the {target.GetComponent<Visual>().DisplayName}.";
            }
            else if (target == ThePlayer)
            {
                missMessage = $"The {attacker.GetComponent<Visual>().DisplayName} misses you.";
            }
            MenuManager.Instance.Log(missMessage);

            return;
        }

        int rawDamage = Dice.Roll(attackDamage);
        int damageAfterArmorSoak = Mathf.Max(0, rawDamage - targetStats["Defense"]);

        if(damageAfterArmorSoak <= 0)
        {
            // tell user failed to penetrate armor
            string armorMessage = $"The {attacker.GetComponent<Visual>().DisplayName} fails to penetrate the {target.GetComponent<Visual>().DisplayName}'s armor.";
            if (attacker == ThePlayer)
            {
                armorMessage = $"You fail to penetrate the {target.GetComponent<Visual>().DisplayName}'s armor.";
            }
            else if (target == ThePlayer)
            {
                armorMessage = $"The {attacker.GetComponent<Visual>().DisplayName} fails to penetrate your armor.";
            }
            MenuManager.Instance.Log(armorMessage);
            return;
        }

        // tell user
        string attackMessage = $"The {attacker.GetComponent<Visual>().DisplayName} attacks the {target.GetComponent<Visual>().DisplayName} for {damageAfterArmorSoak} damage.";
        if(attacker == ThePlayer)
        {
            attackMessage = $"You attack the {target.GetComponent<Visual>().DisplayName} for {damageAfterArmorSoak} damage.";
        }
        else if(target == ThePlayer)
        {
            attackMessage = $"The {attacker.GetComponent<Visual>().DisplayName} attacks you for {damageAfterArmorSoak} damage.";
        }
        MenuManager.Instance.Log(attackMessage);

        targetHealth.HealthCurrent -= damageAfterArmorSoak;
    }
    public void Pickup(GameObject pickerUpper, GameObject item)
    {
        Entities.Remove(item);

        // Destroy() is not instant, it actually occurs at the end of the Update() frame.
        // therefore, i use DestroyImmediate().
        DestroyImmediate(item.GetComponent<Position>());
        
        pickerUpper.GetComponent<Inventory>().AddItem(item);
        pickerUpper.GetComponent<EnergyHaver>().Energy -= Constants.COST_PICKUP;

        if (pickerUpper == ThePlayer)
        {
            StatusMenu.Instance.Log($"You pick up the {item.GetComponent<Visual>().DisplayName}");
        }
        else
        {
            StatusMenu.Instance.Log($"The {pickerUpper.GetComponent<Visual>().DisplayName} picks up the {item.GetComponent<Visual>().DisplayName}");
        }

        // fire an event
        var @event = new PickedUpEvent()
        {
            pickerUpper = pickerUpper
        };
        item.FireEvent(@event);
    }
    public void Drop(GameObject dropper, GameObject toDrop)
    {
        dropper.GetComponent<Inventory>().RemoveItem(toDrop);

        // copy droppers position
        Position newPos = toDrop.gameObject.AddComponent<Position>();
        newPos.Pos = dropper.GetComponent<Position>().Pos;

        Entities.Add(toDrop.gameObject);
    }
    public void DropAll(GameObject dropper)
    {
        // drop all this entities stuff
        Inventory inv = dropper.GetComponent<Inventory>();
        if (inv != null)
        {
            // uneqip everything
            inv.UnequipEverything();
            // drop it all
            while (inv.HasItems())
            {
                Drop(dropper, inv.Items[0]);
            }
        }
    }


    private void CreateNewGameSave()
    {
        gameSave = new GameSave();
    }
}