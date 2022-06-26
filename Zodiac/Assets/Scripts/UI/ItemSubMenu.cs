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

        itemDisplayText.text = item.gameObject.GetComponent<Visual>().DisplayName;

        // add the drop button
        AddButton("Drop", () =>
        {
            GameManager.Drop(item.ContainingInventory.gameObject, item);
            // refresh inv UI to account for the removed item
            Common.inventoryMenu.RefreshUI();
        });
        AddButton("Dupe", () =>
        {
            item.Count *= 2;
            Common.inventoryMenu.RefreshUI();
        }, closeMenuOnUse: false);
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
        if(closeMenuOnUse)
        {
            // add an action to close this menu
            dropButtonComp.onClick.AddListener(() => {
                Common.menuManager.CloseMenu(this);
            });
        }

        return dropButtonComp;
    }
}
