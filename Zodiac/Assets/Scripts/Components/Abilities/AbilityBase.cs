using UI;
using UnityEngine;

public abstract class AbilityBase : ZodiacComponent
{
    [ZodiacNoSerialize]
    public abstract ITargetingMechanism TargetingMechanism { get; }
    [ZodiacNoSerialize]

    public virtual int RechargeTime => 10;
    [ZodiacNoSerialize]
    public virtual int EnergyCost => 1000;
    [ZodiacNoSerialize]
    public virtual string AbilityName => "[AbilityBase]";

    public int Cooldown { get; set; } = 0;
    public virtual void Activate()
    {
        MenuManager.Instance.Log($"{AbilityName} was cast by {gameObject.GetComponent<Visual>().DisplayName}.");
        // deduct energy from user
        gameObject.GetComponent<EnergyHaver>().Energy -= EnergyCost;
        // put ability on cooldown
        Cooldown = RechargeTime;
    }
    public string GetAbilityNameWithCooldown()
    {
        if(Cooldown > 0) {
            return $"{AbilityName} [{Cooldown}]";
        }
        return AbilityName;
    }
}