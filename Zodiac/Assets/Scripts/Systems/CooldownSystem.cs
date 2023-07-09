using System.Linq;
using UnityEngine;

public sealed class CooldownSystem : ISystem
{
	public void Tick()
	{
		foreach (GameObject entity in GameManager.Instance.Entities)
		{
			if (!SelectionCriteria(entity))
			{
				continue;
			}

			foreach(AbilityBase ability in entity.GetComponents<AbilityBase>())
			{
                // reduce cooldowns every tick
                ability.Cooldown -= 1;
                // cap cooldowns at 0
                ability.Cooldown = Mathf.Max(ability.Cooldown, 0);
            }
		}
	}

	private bool SelectionCriteria(GameObject entity)
	{
		return entity.GetComponents<AbilityBase>().Count() > 0;
	}
}