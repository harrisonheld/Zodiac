using UnityEngine;

public class SproutFlower : AbilityBase
{
    private TargetPosition targetPos;
    public override ITargetingMechanism TargetingMechanism => targetPos;
    public override int RechargeTime => 5;
    public override string AbilityName => "Sprout Flower";

    public void Awake()
    {
        targetPos = new TargetPosition(gameObject);
    }

    public override void Activate()
    {
        EntitySerializer.EntityFromBlueprint("Flower", targetPos.GetTargettedPosition());
        StatusMenu.Instance.Log("A flower blooms.");

        base.Activate();
    }
}