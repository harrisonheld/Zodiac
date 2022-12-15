using UnityEngine;
public abstract class ZodiacComponent : MonoBehaviour
{
    public virtual void HandleEvent(ZodiacEvent e)
    {
        // this will call the appropriate HandleEvent for the event type
        dynamic casted = e;
        HandleEvent(casted);
    }
    public virtual bool HandleEvent(PickedUpEvent e) { return false; }
}