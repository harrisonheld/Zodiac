using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSubMenu : MonoBehaviour, IZodiacMenu
{
    [SerializeField] TextMeshProUGUI itemDisplayText;
    [SerializeField] GameObject itemActionPanel;
    [SerializeField] GameObject buttonPrefab;

    private Item item;
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }

    public static ItemSubMenu Instance { get; private set; }
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

    private void Clear()
    {
        foreach (Transform child in itemActionPanel.transform)
        {
            Destroy(child.gameObject);
        }
        itemActionPanel.transform.DetachChildren();
    }

    public void SetItem(Item _item)
    {
        item = _item;
    }

    public void RefreshUI()
    {
        Clear();

        Visual itemVisual = item.gameObject.GetComponent<Visual>();
        itemDisplayText.text = itemVisual.DisplayName;

        // 
        // add buttons
        //

        AddButton("Drop", () =>
        {
            GameManager.Instance.Drop(GameManager.Instance.ThePlayer, item);
            InventoryMenu.Instance.RefreshUI();
        }, closeMenuOnUse: true);
        
        AddButton("Dev Inspect", () =>
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (Component comp in item.gameObject.GetComponents<Component>())
            {
                sb.AppendLine($"<{comp.GetType().Name}>");
                var propInfos = comp.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                foreach (System.Reflection.PropertyInfo propInfo in propInfos)
                {
                    sb.AppendLine($"{propInfo.Name}: {propInfo.GetValue(comp)}");
                }
                sb.AppendLine();
            }

            AlertMenu.Instance.ShowText(sb.ToString());
        }, closeMenuOnUse: false);

        Equippable equippable;
        if (item.gameObject.TryGetComponent<Equippable>(out equippable))
        {
            AddButton("Equip", () =>
            {
                GameManager.Instance.ThePlayer.GetComponent<Inventory>().Equip(equippable);
            }, refreshMenuOnUse: true);
        }

        AddButton("KILL YOURSELF", () => {
            GameManager.Instance.BreakEntity(GameManager.Instance.ThePlayer);
        });
    }
    public void GainFocus()
    {
        if (itemActionPanel.transform.childCount > 0)
        {
            itemActionPanel.transform.GetChild(0).GetComponent<Selectable>().Select();
        }
    }

    private void AddButtonInternal(string text, UnityEngine.Events.UnityAction action, bool closeMenuOnUse, bool refreshMenuOnUse)
    {
        GameObject buttonGo = Instantiate(buttonPrefab, itemActionPanel.transform);
        buttonGo.name = text;
        Button dropButtonComp = buttonGo.GetComponent<Button>();

        buttonGo.GetComponentInChildren<TextMeshProUGUI>().text = text;


        dropButtonComp.onClick.AddListener(() =>
        {
            action();
            if (refreshMenuOnUse)
                RefreshUI();
            if (closeMenuOnUse)
                MenuManager.Instance.Close(this);
        });

    }
    public void AddButton(string text, UnityEngine.Events.UnityAction action, bool closeMenuOnUse = true, bool refreshMenuOnUse = false)
    {
        AddButtonInternal(text, action, closeMenuOnUse, refreshMenuOnUse);
    }
}
