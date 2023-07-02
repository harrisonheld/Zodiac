using UnityEngine;

public sealed class EnergySystem : ISystem
{
	public void Tick()
	{
		foreach (GameObject entity in GameManager.Instance.Entities)
		{
			if (!SelectionCriteria(entity))
			{
				continue;
			}

			// recharge energy every tick
			EnergyHaver energyHaver = entity.GetComponent<EnergyHaver>();
			energyHaver.Energy += energyHaver.Quickness;
			// cap energy at Quickness
			energyHaver.Energy = Mathf.Min(energyHaver.Energy, energyHaver.Quickness); 
		}
	}

	private bool SelectionCriteria(GameObject entity)
	{
		return entity.GetComponent<EnergyHaver>() != null;
	}
}