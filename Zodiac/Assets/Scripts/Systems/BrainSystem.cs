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

            EnergyHaver energyHaver = entity.GetComponent<EnergyHaver>();
			while(energyHaver.Energy > 0)
			{
				bool finished = MakeMove(entity);
                if (finished)
                    break;
			}
		}
	}

    // will return true if the entity wishes to voluntarily end its turn
	private bool MakeMove(GameObject entity)
	{
        Brain brain = entity.GetComponent<Brain>();
        Position myPosComp = brain.gameObject.GetComponent<Position>();
        EnergyHaver energyHaver = entity.GetComponent<EnergyHaver>();
        Vector2Int myPos = myPosComp.Pos;

        switch (brain.Ai)
        {
            case AiType.Seeker:
                {
                    Vector2Int targetPos = GameManager.Instance.ThePlayer.GetComponent<Position>().Pos;
                    Vector2Int towards = targetPos - myPos;
                    Vector2Int delta = towards;
                    delta.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));

                    // if we are next to the target, attack
                    if (towards == delta)
                    {
                        GameManager.Instance.BumpAttack(brain.gameObject, GameManager.Instance.ThePlayer);
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
                    // [-1, 2) = [-1, 1] for integers
                    int moveX = Random.Range(-1, 2);
                    int moveY = Random.Range(-1, 2);
                    Vector2Int randomMove = new Vector2Int(moveX, moveY);
                    Vector2Int moveIntention = myPos + randomMove;
                    GameManager.Instance.AttemptMove(brain.gameObject, moveIntention);
                }
                break;

            case AiType.Projectile:
            case AiType.Inert:
            default:
                break;
        }

        return true;
    }

	private bool SelectionCriteria(GameObject entity)
	{
		return entity.GetComponent<Brain>() != null &&
			entity.GetComponent<Position>() != null &&
			entity.GetComponent<EnergyHaver>() != null &&
			entity != GameManager.Instance.ThePlayer;
	}
}