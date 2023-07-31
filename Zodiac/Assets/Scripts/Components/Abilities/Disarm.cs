using UnityEngine;

public class Disarm : AbilityBase
{
    private TargetEntity targetEntity;
    public override ITargetingMechanism TargetingMechanism => targetEntity;
    public override string AbilityName => "Disarm";

    public void Awake()
    {
        targetEntity = new TargetEntity(gameObject);
    }

    public override void Activate()
    {
        GameObject target = targetEntity.GetTargettedEntity();

        foreach(Slot slot in target.GetComponents<Slot>())
        {
            if(slot.Contained != null && slot.Contained.GetComponent<MeleeWeapon>() != null)
            {
                GameObject weapon = slot.Contained;
                target.GetComponent<Inventory>().UnequipToItems(slot);
                GameManager.Instance.Drop(target, weapon);
                break;
            }
        }

        base.Activate();
    }
}