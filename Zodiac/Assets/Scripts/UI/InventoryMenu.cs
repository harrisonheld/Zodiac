using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryMenu : MonoBehaviour
{
    [SerializeField] GameObject itemsContainer;
    [SerializeField] GameObject itemSlotPrefab;

    private void Start()
    {
        Hide();
    }
    private void Clear()
    {
        // destry all item slots
        foreach (Transform child in itemsContainer.transform)
            Destroy(child.gameObject);
    }
    private void SetInventory(Inventory inventory)
    {
        Clear();

        foreach(Item item in inventory.Contents)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab);
            itemSlot.transform.SetParent(itemsContainer.transform, false);
            itemSlot.GetComponent<ItemSlot>().SetItem(item);
        }
    }

    public void Show(Inventory inventory)
    {
        SetInventory(inventory);
        gameObject.GetComponent<Canvas>().enabled = true;

        ZodiacInput.MenuMode();
    }
    public void Hide()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        ZodiacInput.FreeRoamMode();

        ZodiacInput.FreeRoamMode();
    }
}
