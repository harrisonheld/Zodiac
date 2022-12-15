using UnityEngine;

public abstract class ZodiacComponent : MonoBehaviour
{
    public virtual void HandleEvent(ZodiacEvent e) { }
    public virtual void HandleEvent(PickedUpEvent e) { }
}