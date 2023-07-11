using UnityEngine;

public class Dominate : AbilityBase
{
    private TargetEntity targetEntity;
    public override ITargetingMechanism TargetingMechanism => targetEntity;
    public override string AbilityName => "Dominate";

    public void Awake()
    {
        targetEntity = new TargetEntity(gameObject);
    }

    public override void Activate()
    {
        GameObject target = targetEntity.GetTargettedEntity();
        GameManager.Instance.ThePlayer = target;
        base.Activate();
    }
}