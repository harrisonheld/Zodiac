using UnityEngine;

public class KillFuck : AbilityBase
{
    private TargetEntity targetEntity;
    public override ITargetingMechanism TargetingMechanism => targetEntity;

    public KillFuck()
    {
    }
    public void Awake()
    {
        targetEntity = new TargetEntity(gameObject);
    }

    public override void Activate()
    {
        GameManager.Instance.BreakEntity(targetEntity.GetTargettedEntity());
    }

    public override string GetShortName()
    {
        return "KILL FUCK";
    }
}