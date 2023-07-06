using UnityEngine;

public class Annihilate : AbilityBase
{
    private TargetEntity _targetEntity;
    public override ITargetingMechanism TargetingMechanism => _targetEntity;

    public Annihilate()
    {
        _targetEntity = new TargetEntity();
    }

    public override void Activate()
    {
        _targetEntity.GetTargettedEntity().GetComponent<Health>().HealthCurrent -= 1000;
    }
}