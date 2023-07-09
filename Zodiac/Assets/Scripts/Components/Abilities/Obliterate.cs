using UnityEngine;

public class Obliterate : AbilityBase
{
    private TargetEntity targetEntity;
    public override ITargetingMechanism TargetingMechanism => targetEntity;
    public override string AbilityName => "Obliterate";
    public override int EnergyCost => 10_000;
    public override int RechargeTime => 100;

    public void Awake()
    {
        targetEntity = new TargetEntity(gameObject);
    }

    public override void Activate()
    {
        GameObject target = targetEntity.GetTargettedEntity();
        GameManager.Instance.BreakEntity(target);
        base.Activate();
    }
}