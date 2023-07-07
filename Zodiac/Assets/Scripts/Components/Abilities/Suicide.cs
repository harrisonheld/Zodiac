using UnityEngine;

public class Suicide : AbilityBase
{
    public override ITargetingMechanism TargetingMechanism => new TargetNone();

    public Suicide()
    {

    }

    public override void Activate()
    {
        gameObject.GetComponent<Health>().HealthCurrent = 0;
    }

    public override string GetShortName()
    {
        return "Die Instantaneously";
    }
}