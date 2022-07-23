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

        AddButton("Drop", () =>
        {
            GameManager.Drop(item.ContainingInventory.gameObject, item);
            // refresh inv UI to account for the removed item
            Common.inventoryMenu.RefreshUI();
        });
        AddButton("Inspect", () =>
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            foreach(Component comp in item.gameObject.GetComponents<Component>())
            {
                sb.AppendLine($"<{comp.GetType().Name}>");
                foreach(System.Reflection.FieldInfo fieldInfo in comp.GetType().GetFields())
                {
                    sb.AppendLine($"{fieldInfo.Name}: {fieldInfo.GetValue(comp)}");
                }
                sb.AppendLine();
            }

            Common.alertMenu.SetText(sb.ToString());
            Common.menuManager.Open(Common.alertMenu);
        }, closeMenuOnUse: false);

        Equippable equippable;
        if(item.gameObject.TryGetComponent<Equippable>(out equippable))
        {
            AddButton("Equip", () =>
            {
                ZodiacInput.Player.GetComponent<Inventory>().Equip(equippable);
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

    private Button AddButton(string text, UnityEngine.Events.UnityAction action, bool closeMenuOnUse = true)
    {
        GameObject dropButtonGo = Instantiate(buttonPrefab, itemActionPanel.transform);
        dropButtonGo.name = text;
        Button dropButtonComp = dropButtonGo.GetComponent<Button>();

        dropButtonGo.GetComponentInChildren<TextMeshProUGUI>().text = text;


        dropButtonComp.onClick.AddListener(action);
        if (closeMenuOnUse)
        {
            // add an action to close this menu
            dropButtonComp.onClick.AddListener(() => {
                Common.menuManager.Close(this);
            });
        }

        return dropButtonComp;
    }
}
