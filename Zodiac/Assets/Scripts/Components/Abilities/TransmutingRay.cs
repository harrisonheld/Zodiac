using System.Globalization;
using System.Linq;
using UnityEngine;

public class TransmutingRay : AbilityBase
{
    private TargetEntity targetEntity;
    public override ITargetingMechanism TargetingMechanism => targetEntity;
    public override string AbilityName => new CultureInfo("en-US", false).TextInfo.ToTitleCase($"{MaterialName} Ray");
    public override int EnergyCost => 1_000;
    public override int RechargeTime => 10;

    public string MaterialName { get; set; }
    public Color MaterialColorPrimary { get; set; }
    public Color MaterialColorSecondary { get; set; }
    public Color MaterialColorTertiary { get; set; }

    public void Awake()
    {
        targetEntity = new TargetEntity(gameObject);
    }

    public override void Activate()
    {
        GameObject target = targetEntity.GetTargettedEntity();

        GameManager.Instance.DropAll(target);
        Destroy(target.GetComponent<Brain>());
        Destroy(target.GetComponent<Conversation>());
        Destroy(target.GetComponent<EnergyHaver>());
        Destroy(target.GetComponent<Living>());
        Destroy(target.GetComponent<Inventory>());

        Health health = target.GetComponent<Health>();
        health.HealthMax = 100;
        health.HealthCurrent = 100;

        Visual visual = target.GetComponent<Visual>();
        visual.ColorPrimary = MaterialColorPrimary;
        visual.ColorSecondary = MaterialColorSecondary;
        visual.ColorTertiary = MaterialColorTertiary;

        string oldName = visual.DisplayName;
        visual.DisplayName = $"{MaterialName} statue of {oldName}";
        
        string oldDescription = visual.Description;
        char lastChar = oldDescription[oldDescription.Length - 1];
        if(char.IsPunctuation(lastChar))
        {
            visual.Description += $" It's been turned into {MaterialName}.";
        }
        else
        {
            visual.Description += $". It's been turned into {MaterialName}.";
        }

        base.Activate();
    }
}