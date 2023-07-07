using System.Collections.Generic;
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
        if(!MenuManager.Instance.isOpen(LookMenu.Instance))
        {
            LookMenu.Instance.Show(_targeter.GetComponent<Position>().Pos);
        }

        LookMenu.Instance.HandleInput(inputMap);

        if(inputMap.UI.Submit.triggered)
        {
            _targeted = LookMenu.Instance.GetSubject();
            return true;
        }

        return false;
    }

    public GameObject GetTargettedEntity()
    {
        return _targeted;
    }
}