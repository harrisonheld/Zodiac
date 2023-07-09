using UnityEngine;

public class QuickHeal : AbilityBase
{
    public override ITargetingMechanism TargetingMechanism => new TargetNone();
    public override string AbilityName => "Minor Heal";
    public override void Activate()
    {
        Health h = gameObject.GetComponent<Health>();
        h.HealthCurrent += 5;
        if (h.HealthCurrent > h.HealthMax)
            h.HealthCurrent = h.HealthMax;

    }
}