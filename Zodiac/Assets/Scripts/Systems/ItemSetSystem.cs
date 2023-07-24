using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ItemSetSystem : ISystem
{
	public void Tick()
	{
		foreach (GameObject entity in GameManager.Instance.Entities)
		{
			if (!SelectionCriteria(entity))
			{
				continue;
			}

			foreach(ItemSet set in entity.GetComponents<ItemSet>())
			{
				Inventory inv = entity.GetComponent<Inventory>();
				List<GameObject> items = Raws.ItemSets.SpawnSet(set.ItemSetName);
				foreach(GameObject item in items)
				{
                    inv.AddItem(item);
                }

                GameObject.Destroy(set);
            }
        }
	}

	private bool SelectionCriteria(GameObject entity)
	{
		return entity.GetComponents<ItemSet>().Count() > 0;
	}
}