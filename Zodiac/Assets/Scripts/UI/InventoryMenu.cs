using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryMenu : MonoBehaviour, IZodiacMenu
{
    [SerializeField] GameObject itemsContainer;
    [SerializeField] GameObject itemSlotPrefab;

    private Inventory inventory;
    public Canvas Canvas { get => GetComponent<Canvas>(); }
    public CanvasGroup CanvasGroup { get => GetComponent<CanvasGroup>(); }
    public static InventoryMenu Instance { get; private set; }
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

    public void RefreshUI()
    {
        Clear();

        for (int i = 0; i < inventory.Items.Count; i++)
        {
            Item item = inventory.Items[i];
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
