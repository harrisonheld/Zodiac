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
    }

    private void Update()
    {
        // closing of menu
        if (ZodiacInput.InputMap.UI.Cancel.triggered)
        {
            Close();
        }
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

        for(int i = 0; i < inventory.Contents.Count; i++)
        {
            Item item = inventory.Contents[i];
            GameObject itemSlot = Instantiate(itemSlotPrefab);
            itemSlot.transform.SetParent(itemsContainer.transform, false);
            itemSlot.GetComponent<ItemSlot>().SetItem(item);
            itemSlot.gameObject.name = $"Slot {i}";

            if(i == 0)
            {
                itemSlot.GetComponent<Selectable>().Select();
            }
        }
    }

    public void Show(Inventory inventory)
    {
        SetInventory(inventory);

        var canvas = gameObject.GetComponent<Canvas>();
        canvas.enabled = true;

        ZodiacInput.MenuMode();
    }
    public void Close()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        ZodiacInput.FreeRoamMode();

        ZodiacInput.FreeRoamMode();
    }
}
