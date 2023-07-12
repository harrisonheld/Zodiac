using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class TargetPosition : ITargetingMechanism
{
    private GameObject _targeter = null;
    private Vector2Int _targeted = Constants.OFFSCREEN;

    private bool _targetPosMustBeEmpty = true;

    public TargetPosition(GameObject targeter, bool targetPosMustBeEmpty = true)
    {
        _targeter = targeter;
        _targetPosMustBeEmpty = targetPosMustBeEmpty;
    }

    public bool HandleInput(ZodiacInputMap inputMap)
    {
        if(!LookMenu.Instance.isOpen)
        {
            LookMenu.Instance.Show(GameManager.Instance.ThePlayer);
        }

        LookMenu.Instance.HandleInput(inputMap);

        if(inputMap.UI.Submit.triggered)
        {
            _targeted = LookMenu.Instance.GetCursorPos();

            if(_targetPosMustBeEmpty)
            {
                // return true only if there are no solid entities at the target position
                bool posEmpty = GameManager.Instance.EntitiesAt(_targeted)
                    .Where(e => e.GetComponent<PhysicalAttributes>().Solid)
                    .Count() == 0;

                if(!posEmpty)
                {
                    MenuManager.Instance.Log("There is something in the way.");
                }

                return posEmpty;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    public Vector2Int GetTargettedPosition()
    {
        return _targeted;
    }
}