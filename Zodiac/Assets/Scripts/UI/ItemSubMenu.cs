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

        AddButtonInternal("Drop", () =>
        {
            GameManager.Instance.Drop(GameManager.Instance.ThePlayer, item);
            // refresh inv UI to account for the removed item
            Common.inventoryMenu.RefreshUI();
        });
        AddButtonInternal("Inspect", () =>
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (Component comp in item.gameObject.GetComponents<Component>())
            {
                sb.AppendLine($"<{comp.GetType().Name}>");
                foreach (System.Reflection.FieldInfo fieldInfo in comp.GetType().GetFields())
                {
                    sb.AppendLine($"{fieldInfo.Name}: {fieldInfo.GetValue(comp)}");
                }
                sb.AppendLine();
            }

            Common.alertMenu.ShowText(sb.ToString());
        }, closeMenuOnUse: false);

        Equippable equippable;
        if (item.gameObject.TryGetComponent<Equippable>(out equippable))
        {
            AddButtonInternal("Equip", () =>
            {
                GameManager.Instance.ThePlayer.GetComponent<Inventory>().Equip(equippable);
                // refresh inv UI to account for the removed item
                Common.inventoryMenu.RefreshUI();
            });
        }
    }
    public void GainFocus()
    {
        if (itemActionPanel.transform.childCount > 0)
        {
            itemActionPanel.transform.GetChild(0).GetComponent<Selectable>().Select();
        }
    }

    private void AddButtonInternal(string text, UnityEngine.Events.UnityAction action, bool closeMenuOnUse = true)
    {
        GameObject buttonGo = Instantiate(buttonPrefab, itemActionPanel.transform);
        buttonGo.name = text;
        Button dropButtonComp = buttonGo.GetComponent<Button>();

        buttonGo.GetComponentInChildren<TextMeshProUGUI>().text = text;


        dropButtonComp.onClick.AddListener(action);
        if (closeMenuOnUse)
        {
            // add an action to close this menu
            dropButtonComp.onClick.AddListener(() => {
                Common.menuManager.Close(this);
            });
        }
    }
    public void AddButton(string text, UnityEngine.Events.UnityAction action, bool closeMenuOnUse = true)
    {
        AddButtonInternal(text, action, closeMenuOnUse);
    }
}
