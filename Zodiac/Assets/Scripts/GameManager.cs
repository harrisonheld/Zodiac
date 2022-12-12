using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] int turn = 0;

    public List<GameObject> Entities = new List<GameObject>();
    [SerializeField] public GameObject ThePlayer;

    // a reference to the one game manager in the scene
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
        // get stuff
        Common.menuManager = GetComponent<MenuManager>();
        Common.inventoryMenu = GameObject.Find("InventoryMenu").GetComponent<InventoryMenu>();
        Common.itemSubMenu = GameObject.Find("ItemSubMenu").GetComponent<ItemSubMenu>();
        Common.alertMenu = GameObject.Find("AlertMenu").GetComponent<AlertMenu>();
        Common.cursor = GameObject.Find("Cursor");
        Common.lookMenu = GameObject.Find("LookMenu").GetComponent<LookMenu>();

        // get all entities
        foreach (Position posComp in GameObject.FindObjectsOfType<Position>())
        {
            Entities.Add(posComp.gameObject);
        }
    }

    public void Update()
    {
        // death
        if(ThePlayer == null)
        {
            // tell the player this
            if(!AlertMenu.Instance.Canvas.isActiveAndEnabled)
            {
                MenuManager.Instance.CloseAll();
                AlertMenu.Instance.ShowText("Unfortunately, you have died or otherwise ceased to exist.");
            }
            return;
        }

        bool inputDone = ZodiacInput.DoPlayerInput();
        if (inputDone)
            DoTurn();
    }
    private void DoTurn()
    {
        EnergySystem();
        AbilitySystem();
        BrainSystem();

        turn++;
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
    public void BreakEntity(GameObject toDestroy)
    {
        // drop all this entities stuff
        Inventory inv = toDestroy.GetComponent<Inventory>();
        if(inv != null)
        {
            // uneqip everything
            inv.UnequipEverything();
            // drop it all
            while(inv.HasItems())
            {
                Drop(toDestroy, inv.Items[0]);
            }
        }

        // bye bye!
        Entities.Remove(toDestroy);
        GameObject.Destroy(toDestroy);
    }
    public bool isValidMovePosition(Vector2Int toCheck)
    {
        // position is valid if nothing solid is here
        foreach(GameObject entity in EntitiesAt(toCheck))
        {
            if (entity.GetComponent<PhysicalAttributes>().Solid)
                return false;
        }

        return true;
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
            Debug.Log("Failed to attack as target has no health component.");
            return;
        }

        // calculate attack stats
        int attackDamage = 1;
        int attackCost = 1000; // cost of the attack in energy

        Equippable attackerPrimary = attacker.GetComponent<Inventory>().GetPrimary();
        if(attackerPrimary != null)
        {
            MeleeWeapon weapon = attackerPrimary.GetComponent<MeleeWeapon>();
            if(weapon != null)
            {
                attackDamage = weapon.Damage;
                attackCost = weapon.AttackCost;
            }
        }

        attacker.GetComponent<EnergyHaver>().Energy -= attackCost;

        targetHealth.HealthCurrent -= Mathf.Max(0, attackDamage - targetHealth.Defense);
    }
    public void Pickup(GameObject pickerUpper, Item item)
    {
        Entities.Remove(item.gameObject);

        // destroy pos, if it has one
        Position posComp = item.GetComponent<Position>();
        // Destroy() is not instant, it actually occurs at the end of the Update() frame.
        // therefore, i use DestroyImmediate().
        DestroyImmediate(posComp);
        
        pickerUpper.GetComponent<Inventory>().AddItem(item);
        pickerUpper.GetComponent<EnergyHaver>().Energy -= Constants.COST_PICKUP;
        Debug.Log("Picked up " + item.name);
    }
    public void Drop(GameObject dropper, Item toDrop)
    {
        if (toDrop.name == "Bow")
        {
            // do nothing yay
        }

        dropper.GetComponent<Inventory>().RemoveItem(toDrop);

        // copy droppers position
        Position newPos = toDrop.gameObject.AddComponent<Position>();
        newPos.Pos = dropper.GetComponent<Position>().Pos;

        Entities.Add(toDrop.gameObject);
    }

    public static int Distance(Vector2Int a, Vector2Int b)
    {
        /* 
         * d = Max( |delta X|, |delta Y| )
         * This makes all 8 adjacent tiles equadistant.
        */
        
        int changeInX = a.x - b.x;
        int changeInY = a.y - b.y;
        return Mathf.Max(Mathf.Abs(changeInX), Mathf.Abs(changeInY));
    }


    private void EnergySystem()
    {
        foreach (EnergyHaver energyHaver in GameObject.FindObjectsOfType<EnergyHaver>())
        {
            energyHaver.Energy += energyHaver.Quickness;
            energyHaver.Energy = Mathf.Min(energyHaver.Energy, energyHaver.Quickness); // cap energy at Quickness
        }
    }
    private void BrainSystem()
    {
        foreach (Brain brain in Object.FindObjectsOfType<Brain>()) 
        {
            // check if null in case another AI's action caused this object to be destroyed
            if (brain == null)
                continue;

            // check if this entity is on the map
            if (!Entities.Contains(brain.gameObject))
                continue;
            if (brain.gameObject == null)
                continue;

            Position myPosComp = brain.gameObject.GetComponent<Position>();
            EnergyHaver energyHaver = brain.gameObject.GetComponent<EnergyHaver>();

            Vector2Int myPos = myPosComp.Pos;

            switch (brain.Ai) 
            {
                case AiType.Seeker:
                    {
                        if (energyHaver.Energy <= 0)
                            break;

                        Vector2Int targetPos = brain.Target.GetComponent<Position>().Pos;
                        Vector2Int towards = targetPos - myPos;
                        Vector2Int delta = towards;
                        delta.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));

                        // if we are next to the target, attack
                        if(towards == delta)
                        {
                            BumpAttack(brain.gameObject, brain.Target);

                            // if target was killed
                            if (brain.Target == null)
                                brain.Ai = AiType.Wanderer;
                        }
                        else
                        {
                            Vector2Int moveIntention = myPos + delta;
                            AttemptMove(brain.gameObject, moveIntention);
                        }
                    }
                    break;

                case AiType.Wanderer:
                    {
                        if (energyHaver.Energy <= 0)
                            break;

                        // [-1, 2) implies [-1, 1]
                        int moveX = Random.Range(-1, 2);
                        int moveY = Random.Range(-1, 2);
                        Vector2Int randomMove = new Vector2Int(moveX, moveY);
                        Vector2Int moveIntention = myPos + randomMove;
                        AttemptMove(brain.gameObject, moveIntention);
                    }
                    break;

                case AiType.Projectile:
                    {
                        if (energyHaver.Energy <= 0)
                            break;
                    }
                    break;


                case AiType.Inert:
                default:
                    break;
            }
        }
    }
    private void AbilitySystem()
    {
        foreach (Ability ability in GameObject.FindObjectsOfType<Ability>())
        {
            if (ability.Cooldown > 0)
                ability.Cooldown--;
        }
    }
}