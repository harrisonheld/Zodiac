using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] int tick = 0;

    public static List<GameObject> Entities = new List<GameObject>();

    public void Start()
    {
        // get stuff
        Common.menuManager = GetComponent<MenuManager>();
        Common.inventoryMenu = GameObject.Find("InventoryMenu").GetComponent<InventoryMenu>();
        Common.itemSubMenu = GameObject.Find("ItemSubMenu").GetComponent<ItemSubMenu>();
        Common.alertMenu = GameObject.Find("AlertMenu").GetComponent<AlertMenu>();

        // get all entities
        foreach (Physical phys in GameObject.FindObjectsOfType<Physical>())
        {
            Entities.Add(phys.gameObject);
        }

        // start tick loop
        StartCoroutine(TickLoop());
    }
    private IEnumerator TickLoop()
    {
        while(true)
        {
            yield return StartCoroutine(Tick());
            tick++;
            yield return null;
        }
    }
    private IEnumerator Tick()
    {
        yield return StartCoroutine(EnergySystem());
        yield return StartCoroutine(BrainSystem());
        yield return StartCoroutine(HealthSystem());
        yield return null;
    }



    /// <summary>
    /// Get the entity at the specified position.
    /// </summary>
    public static GameObject EntityAt(Vector2Int pos)
    {
        foreach(GameObject entity in Entities)
        {
            if (entity.GetComponent<Physical>().Position == pos)
                return entity;
        }

        return null;
    }
    public static List<GameObject> EntitiesAt(Vector2Int pos)
    {
        var result = new List<GameObject>();

        foreach (GameObject entity in Entities)
        {
            if (entity.GetComponent<Physical>().Position == pos)
                result.Add(entity);
        }

        return result;
    }
    public static void Destroy(GameObject toDestroy)
    {
        Entities.Remove(toDestroy);
        GameObject.Destroy(toDestroy);
    }
    public static bool ValidMovePosition(Vector2Int toCheck)
    {
        // position is valid if nothing solid is here
        foreach(GameObject entity in EntitiesAt(toCheck))
        {
            if (entity.GetComponent<Physical>().Solid)
                return false;
        }

        return true;
    }

    public static void Attack(GameObject attacker, GameObject target)
    {
        var health = target.GetComponent<Health>();
        if (health == null)
        {
            Debug.Log("Failed to attack as target has no health component.");
            return;
        }
        attacker.GetComponent<EnergyHaver>().Energy -= Constants.COST_MOVE;
        health.CurrentHealth -= Mathf.Max(0, 4 - health.Defense);
    }
    public static void Pickup(GameObject pickerUpper, Item item)
    {
        Entities.Remove(item.gameObject);
        pickerUpper.GetComponent<Inventory>().AddItem(item);
    }
    public static void Drop(GameObject dropper, Item toDrop)
    {
        dropper.GetComponent<Inventory>().RemoveItem(toDrop);
        // copy droppers position
        toDrop.gameObject.AddComponent<Physical>()
            .Position = dropper.GetComponent<Physical>().Position;

        Entities.Add(toDrop.gameObject);
    }
    public static void Move(GameObject toMove, Vector2Int destination)
    {
        toMove.GetComponent<EnergyHaver>().Energy -= Constants.COST_MOVE;
        toMove.GetComponent<Physical>().Position = destination;
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


    private IEnumerator EnergySystem()
    {
        foreach (EnergyHaver energyHaver in GameObject.FindObjectsOfType<EnergyHaver>())
        {
            energyHaver.Energy += energyHaver.Speed;

            if (energyHaver.Energy >= Constants.ENERGY_CAP)
                energyHaver.Energy = Constants.ENERGY_CAP;
        }

        yield return null;
    }
    private IEnumerator BrainSystem()
    {
        foreach (Brain brain in Object.FindObjectsOfType<Brain>()) 
        {

            if (!Entities.Contains(brain.gameObject))
                continue;

            Physical myPhys = brain.gameObject.GetComponent<Physical>();
            EnergyHaver energyHaver = brain.gameObject.GetComponent<EnergyHaver>();

            // any entity that has more than 0 energy can do whatever move they want
            // if it costs too much, they will simply go into the negative and be unable to do anything
            // until they are above zero energy
            if (energyHaver.Energy < Constants.COST_MOVE) 
                continue;

            Vector2Int myPos = myPhys.Position;

            switch (brain.Ai) 
            {
                case AiType.PlayerControlled:
                    {
                        yield return StartCoroutine(ZodiacInput.DoPlayerInput(brain.gameObject));
                    }
                    break;

                case AiType.Seeker:
                    {
                        Vector2Int targetPos = brain.Target.GetComponent<Physical>().Position;
                        Vector2Int delta = targetPos - myPos;

                        delta.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));
                        Vector2Int moveIntention = myPos + delta;
                        bool valid = ValidMovePosition(moveIntention);
                        if(valid)
                            Move(brain.gameObject, moveIntention);
                    }
                break;

                case AiType.Wanderer:
                    {
                        // [-1, 2) implies [-1, 1]
                        int moveX = Random.Range(-1, 2);
                        int moveY = Random.Range(-1, 2);
                        Vector2Int randomMove = new Vector2Int(moveX, moveY);
                        Vector2Int moveIntention = myPos + randomMove;
                        Move(brain.gameObject, moveIntention);
                    }
                break;


                case AiType.Inert:
                default:
                    break;
            }
        }

        yield return null;
    }
    private IEnumerator HealthSystem()
    {
        foreach (Health health in Object.FindObjectsOfType<Health>())
        {
            if (health.CurrentHealth <= 0)
                Destroy(health.gameObject);
        }

        yield return null;
    }
}