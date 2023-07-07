using UnityEngine;

public class LifeFuck : AbilityBase
{
    private TargetPosition targetPos;
    public override ITargetingMechanism TargetingMechanism => targetPos;

    public LifeFuck()
    {
    }
    public void Awake()
    {
        targetPos = new TargetPosition(gameObject);
    }

    public override void Activate()
    {
        StatusMenu.Instance.Log("You cast LIFE FUCK.");
        EntitySerializer.EntityFromBlueprint("Flower", targetPos.GetTargettedPosition());
        StatusMenu.Instance.Log("A flower blooms.");
    }

    public override string GetShortName()
    {
        return "LIFE FUCK";
    }
}