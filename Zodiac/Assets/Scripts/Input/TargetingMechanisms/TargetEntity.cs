using System.Collections.Generic;
using UI;
using UnityEngine;

public class TargetEntity : ITargetingMechanism
{
    private GameObject _targeter = null;
    private GameObject _targeted = null;

    public TargetEntity(GameObject targeter)
    {
        _targeter = targeter;
        _targeted = null;
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
            _targeted = LookMenu.Instance.GetSubject();

            if(_targeted == null)
            {
                MenuManager.Instance.Log("There is nothing to target there.");
                return false;
            }

            return true;
        }

        return false;
    }

    public GameObject GetTargettedEntity()
    {
        return _targeted;
    }
}