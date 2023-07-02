using UnityEngine;

public sealed class BrainSystem : ISystem
{
	public void Tick()
	{
		foreach (GameObject entity in GameManager.Instance.Entities)
		{
			if(!SelectionCriteria(entity))
			{
				continue;
			}

			Brain brain = entity.GetComponent<Brain>();
			Position myPosComp = brain.gameObject.GetComponent<Position>();
			EnergyHaver energyHaver = brain.gameObject.GetComponent<EnergyHaver>();

			Vector2Int myPos = myPosComp.Pos;

			switch (brain.Ai)
			{
				case AiType.Seeker:
					{
						if (energyHaver.Energy <= 0)
							break;
						if (brain.Target == null)
							break;

						Vector2Int targetPos = brain.Target.GetComponent<Position>().Pos;
						Vector2Int towards = targetPos - myPos;
						Vector2Int delta = towards;
						delta.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));

						// if we are next to the target, attack
						if (towards == delta)
						{
							GameManager.Instance.BumpAttack(brain.gameObject, brain.Target);

							// if target was killed
							if (brain.Target == null)
								brain.Ai = AiType.Wanderer;
						}
						else
						{
							Vector2Int moveIntention = myPos + delta;
							GameManager.Instance.AttemptMove(brain.gameObject, moveIntention);
						}
					}
					break;

				case AiType.Wanderer:
					{
						if (energyHaver.Energy <= 0)
							break;

						// [-1, 2) = [-1, 1] for integers
						int moveX = Random.Range(-1, 2);
						int moveY = Random.Range(-1, 2);
						Vector2Int randomMove = new Vector2Int(moveX, moveY);
						Vector2Int moveIntention = myPos + randomMove;
						GameManager.Instance.AttemptMove(brain.gameObject, moveIntention);
					}
					break;

				case AiType.Projectile:
					{
						if (energyHaver.Energy <= 0)
							break;
					}
					break;


				case AiType.Inert:
				default:
					break;
			}
		}
	}

	private bool SelectionCriteria(GameObject entity)
	{
		return entity.GetComponent<Brain>() != null &&
			entity.GetComponent<Position>() != null &&
			entity.GetComponent<EnergyHaver>() != null &&
			entity != GameManager.Instance.ThePlayer;
	}
}