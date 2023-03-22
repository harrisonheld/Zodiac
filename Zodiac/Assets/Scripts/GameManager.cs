using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GameManager : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private int turn = 0;
    [SerializeField] private int screenX = 2;
    [SerializeField] private int screenY = 15;

    private GameSave gameSave;

    // a list of Entities that have Position components
    public List<GameObject> Entities = new List<GameObject>();
    public List<GameObject>[,] EntitiesByPosition = new List<GameObject>[WorldGen.World.SCREEN_WIDTH, WorldGen.World.SCREEN_HEIGHT];

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
        CreateNewGameSave();


        // initialize EntitiesByPosition
        for (int x = 0; x < WorldGen.World.SCREEN_WIDTH; x++)
            for (int y = 0; y < WorldGen.World.SCREEN_HEIGHT; y++)
                EntitiesByPosition[x, y] = new List<GameObject>();


        string testpath = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Entities\moonshinercave.xml";
        EntitySerializer serializer = new();
        List < GameObject> deserialized = serializer.DeserializeScene(testpath);
        ThePlayer = deserialized[1];

        
        WorldGen.World.SetWorldSeed(gameSave.WorldSeed);
        Entities.AddRange(deserialized.Where(e => e.GetComponents<Position>() != null));
        Entities.AddRange(WorldGen.World.GenerateScreen(screenX, screenY));
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
                AlertMenu.Instance.ShowText("Unfortunately, you have died or otherwise ceased to exist.");
            }
            return;
        }

        bool inputDone = ZodiacInput.DoPlayerInput();
        if (inputDone)
        {
            DoTurn();

            Position playerPos = ThePlayer.GetComponent<Position>();
            bool leftScreen = false;
            if (playerPos.Y >= WorldGen.World.SCREEN_HEIGHT)
            {
                playerPos.Y %= WorldGen.World.SCREEN_HEIGHT;
                gameSave.SaveScreen(screenX, screenY);
                screenY++;
                leftScreen = true;
            }
            else if(playerPos.Y < 0)
            {
                playerPos.Y += WorldGen.World.SCREEN_HEIGHT;
                gameSave.SaveScreen(screenX, screenY);
                screenY--;
                leftScreen = true;
            }
            if (playerPos.X >= WorldGen.World.SCREEN_WIDTH)
            {
                playerPos.X %= WorldGen.World.SCREEN_WIDTH;
                gameSave.SaveScreen(screenX, screenY);
                screenX++;
                leftScreen = true;
            }
            else if (playerPos.X < 0)
            {
                playerPos.X += WorldGen.World.SCREEN_WIDTH;
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
                
                if(gameSave.ScreenSaved(screenX, screenY))
                    Entities.AddRange(gameSave.LoadScreen(screenX, screenY));
                else
                    Entities.AddRange(WorldGen.World.GenerateScreen(screenX, screenY));
            }
        }
    }
    private void DoTurn()
    {
        EnergySystem();
        BrainSystem();

        turn++;
    }
    public int GetTurn()
    {
        return turn;
    }



    /// <summary>
    /// Get the entity at the specified position.
    /// </summary>
    public GameObject EntityAt(Vector2Int pos)
    {
        foreach(GameObject entity in Entities)
        {
            if(entity == null)
                continue;
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
            if (entity == null)
                continue;
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

        // tell user
        string deathMessage = $"The {toDestroy.GetComponent<Visual>().DisplayName} is destroyed.";
        if (toDestroy.GetComponent<Living>() != null)
            deathMessage = $"The {toDestroy.GetComponent<Visual>().DisplayName} dies.";
        StatusMenu.Instance.Log(deathMessage);

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

        // get the equippable in the first slot
        GameObject attackerPrimary = attacker.GetComponent<Slot>()?.Contained;
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
    public void Pickup(GameObject pickerUpper, GameObject item)
    {
        Entities.Remove(item);

        // destroy pos, if it has one
        Position posComp = item.GetComponent<Position>();
        // Destroy() is not instant, it actually occurs at the end of the Update() frame.
        // therefore, i use DestroyImmediate().
        DestroyImmediate(posComp);
        
        pickerUpper.GetComponent<Inventory>().AddItem(item);
        pickerUpper.GetComponent<EnergyHaver>().Energy -= Constants.COST_PICKUP;

        var @event = new PickedUpEvent()
        {
            pickerUpper = pickerUpper
        };
        item.FireEvent(@event);

        if (pickerUpper == ThePlayer)
        {
            StatusMenu.Instance.Log($"You pick up the {item.GetComponent<Visual>().DisplayName}");
        }
        else
        {
            StatusMenu.Instance.Log($"The {pickerUpper.GetComponent<Visual>().DisplayName} picks up the {item.GetComponent<Visual>().DisplayName}");
        }
    }
    public void Drop(GameObject dropper, GameObject toDrop)
    {
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
            if (brain.gameObject == null)
                continue;
            // check if this entity is on the map
            if (!Entities.Contains(brain.gameObject))
                continue;
            // if player controlled, dont do its AI
            if (brain.gameObject == ThePlayer)
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
                        if (brain.Target == null)
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


    private void CreateNewGameSave()
    {
        gameSave = new GameSave();
    }
}