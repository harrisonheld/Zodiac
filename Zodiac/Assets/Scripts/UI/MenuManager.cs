using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _pickMenuPrefab;
        [SerializeField] private GameObject _alertMenuPrefab;
        [SerializeField] private GameObject _itemSubMenuPrefab;
        [SerializeField] private GameObject _inventoryPrefab;
        [SerializeField] private GameObject _conversationMenuPrefab;
        [SerializeField] private GameObject _questMenuPrefab;

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
        private static List<IZodiacMenu> _menus = new List<IZodiacMenu>();

        public void Update()
        {
            if (!AnyMenusOpen())
                return;

            // closing of menus
            if (ZodiacInput.InputMap.UI.Cancel.triggered)
                CloseTopMenu();
        }

        public IZodiacMenu TopMenu()
        {
            return _menus.Last();
        }
        public void CloseTopMenu()
        {
            Close(TopMenu());
        }
        public void CloseAll()
        {
            while (_menus.Count > 0)
                Close(TopMenu());
        }
        public void RefreshUIs()
        {
            foreach (IZodiacMenu menu in _menus)
                menu.RefreshUI();
        }

        public void Open(IZodiacMenu menu)
        {
            if (AnyMenusOpen())
            {
                // disable interaction on previous menu
                TopMenu().CanvasGroup.interactable = false;
            }

            menu.RefreshUI();
            menu.GainFocus();
            _menus.Add(menu);
        }
        public void Close(IZodiacMenu toClose)
        {
            _menus.Remove(toClose);
            Destroy(toClose.GameObject);

            if (AnyMenusOpen())
            {
                TopMenu().CanvasGroup.interactable = true;
                TopMenu().GainFocus();
            }
        }

        public bool isOpen(IZodiacMenu toCheck)
        {
            return toCheck.Canvas.enabled;
        }
        public bool AnyMenusOpen()
        {
            return _menus.Count > 0;
        }

        public void SetStatusMenuSize(Vector2 size)
        {
            StatusMenu.Instance.SetSize(size);
        }

        public void Log(string info)
        {
            StatusMenu.Instance.Log(info);
        }
        public void LogMiss(GameObject attacker, GameObject target)
        {
            // tell user about the miss
            string missMessage = $"The {attacker.GetComponent<Visual>().DisplayName} misses the {target.GetComponent<Visual>().DisplayName}.";
            if (attacker == GameManager.Instance.ThePlayer)
            {
                missMessage = $"You miss the {target.GetComponent<Visual>().DisplayName}.";
            }
            else if (target == GameManager.Instance.ThePlayer)
            {
                missMessage = $"The {attacker.GetComponent<Visual>().DisplayName} misses you.";
            }
            StatusMenu.Instance.Log(missMessage);
        }
        public void LogAttack(GameObject attacker, GameObject target, int damage)
        {
            // tell user about the damage
            string damageMessage = $"The {attacker.GetComponent<Visual>().DisplayName} hits the {target.GetComponent<Visual>().DisplayName} for {damage} damage.";
            if (attacker == GameManager.Instance.ThePlayer)
            {
                damageMessage = $"You hit the {target.GetComponent<Visual>().DisplayName} for {damage} damage.";
            }
            else if (target == GameManager.Instance.ThePlayer)
            {
                damageMessage = $"The {attacker.GetComponent<Visual>().DisplayName} hits you for {damage} damage.";
            }
            StatusMenu.Instance.Log(damageMessage);
        }
        public void LogDeath(GameObject dearlyDeparted)
        {
            // tell user about the death
            string deathMessage = $"The {dearlyDeparted.GetComponent<Visual>().DisplayName} is destroyed.";
            if (dearlyDeparted == GameManager.Instance.ThePlayer)
            {
                deathMessage = $"You die.";
            }
            else if (dearlyDeparted.GetComponent<Living>() != null)
            {
                deathMessage = $"The {dearlyDeparted.GetComponent<Visual>().DisplayName} dies.";
            }
            
            StatusMenu.Instance.Log(deathMessage);
        }
        public void LogArmorChink(GameObject attacker, GameObject target)
        {
            string armorMessage = $"The {attacker.GetComponent<Visual>().DisplayName} hits the {target.GetComponent<Visual>().DisplayName}, but the attack fails to penetrate.";
            if (attacker == GameManager.Instance.ThePlayer)
            {
                armorMessage = $"You hit the {target.GetComponent<Visual>().DisplayName}, but the attack fails to penetrate.";
            }
            else if (target == GameManager.Instance.ThePlayer)
            {
                armorMessage = $"The {attacker.GetComponent<Visual>().DisplayName} hits you, but the attack fails to penetrate.";
            }
            StatusMenu.Instance.Log(armorMessage);
        }
        public void LogPickup(GameObject pickerUpper, GameObject item)
        {
            string pickupMessage;
            if (pickerUpper == GameManager.Instance.ThePlayer)
            {
                pickupMessage = $"You pick up the {item.GetComponent<Visual>().DisplayName}";
            }
            else
            {
                pickupMessage = $"The {pickerUpper.GetComponent<Visual>().DisplayName} picks up the {item.GetComponent<Visual>().DisplayName}";
            }
            StatusMenu.Instance.Log(pickupMessage);
        }

        public void ShowConversation(string startNode, GameObject speaker)
        {
            ConversationMenu menu = Instantiate(_conversationMenuPrefab).GetComponent<ConversationMenu>();
            menu.GoToNode(startNode);
            menu.SetSpeaker(speaker);
            Open(menu);
        }

        public void ShowAlert(string text)
        {
            AlertMenu menu = Instantiate(_alertMenuPrefab).GetComponent<AlertMenu>();
            menu.SetText(text);
            Open(menu);
        }
        public void ShowItemSubMenu(GameObject item)
        {
            ItemSubMenu menu = Instantiate(_itemSubMenuPrefab).GetComponent<ItemSubMenu>();
            menu.SetItem(item);
            Open(menu);
        }
        public void ShowInventory(Inventory inv)
        {
            InventoryMenu menu = Instantiate(_inventoryPrefab).GetComponent<InventoryMenu>();
            menu.SetInventory(inv);
            Open(menu);
        }
        public void ShowQuestMenu()
        {
            QuestMenu menu = Instantiate(_questMenuPrefab).GetComponent<QuestMenu>();
            Open(menu);
        }

        public void ShowPickMenu<T>(IList<T> options,
                       Action<T> action,
                       Func<T, string> getName = null,
                       Func<T, bool> criterion = null,
                       string prompt = "Pick",
                       bool closeOnPick = true,
                       bool removeOnPick = true)
        {
            PickMenu menu = Instantiate(_pickMenuPrefab).GetComponent<PickMenu>();
            menu.Pick(options, action, getName, criterion, prompt, closeOnPick, removeOnPick);
            Open(menu);
        }
        public void EquipmentMenu()
        {
            ShowPickMenu(
                options: GameManager.Instance.ThePlayer.GetComponents<Slot>().OrderBy(s => s.SlotType).ToList(),
                action: slot =>
                {
                    if(slot.Empty)
                    {
                        ShowPickMenu(
                            GameManager.Instance.ThePlayer.GetComponent<Inventory>().Items,
                            item =>
                            {
                                GameManager.Instance.ThePlayer.GetComponent<Inventory>().Equip(item, slot);
                                RefreshUIs();
                            },
                            criterion: item => item.GetComponent<Equippable>() && item.GetComponent<Equippable>().SlotType == slot.SlotType,
                            getName: item => item.GetComponent<Visual>().DisplayName,
                            prompt: "Items",
                            closeOnPick: true
                        );
                    }
                    else
                    {
                        GameManager.Instance.ThePlayer.GetComponent<Inventory>().UnequipToItems(slot);
                        RefreshUIs();
                    }
                },
                getName: slot => slot.GetNameWithItem(),
                prompt: "Equipment",
                closeOnPick: false,
                removeOnPick: false
            );
        }
    }
}