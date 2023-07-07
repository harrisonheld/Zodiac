using UnityEngine;

public abstract class AbilityBase : ZodiacComponent
{
    [ZodiacNoSerialize]
    public abstract ITargetingMechanism TargetingMechanism { get; }
    public virtual bool isReady { get; } = true;
    public abstract void Activate();

    public abstract string GetShortName();
}