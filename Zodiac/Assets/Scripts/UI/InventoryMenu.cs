using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
class InventoryMenu : MonoBehaviour, IZodiacMenu
{
    [SerializeField] GameObject itemsContainer;
    [SerializeField] GameObject itemSlotPrefab;

    private Inventory inventory;
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }
    public GameObject GameObject { get => gameObject; }

    public void RefreshUI()
    {
        Clear();

        for (int i = 0; i < inventory.Items.Count; i++)
        {
            GameObject item = inventory.Items[i];
            GameObject itemSlot = Instantiate(itemSlotPrefab);
            itemSlot.transform.SetParent(itemsContainer.transform, false);
            itemSlot.GetComponent<ItemSlot>().SetItem(item);
            itemSlot.gameObject.name = $"Slot {i}";
        }
    }
    public void GainFocus()
    {
        if(itemsContainer.transform.childCount > 0)
        {
            itemsContainer.transform.GetChild(0).GetComponent<Selectable>().Select();
        }
    }
    private void Clear()
    {
        // mark all children for destruction
        foreach (Transform child in itemsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        // detach children now, as they may not be destroyed instantly
        itemsContainer.transform.DetachChildren();
    }
    public void SetInventory(Inventory _inventory)
    {
        inventory = _inventory;
    }
}
