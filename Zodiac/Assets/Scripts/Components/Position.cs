using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Position : ZodiacComponent
{
    [SerializeField] private Vector2Int pos;

    private Coroutine visualLerpCoroutine;

    [ZodiacNoSerialize]
    public Vector2Int Pos
    {
        get => pos;
        set
        {
            pos = value;
            // update transform position
            gameObject.transform.position = (Vector2)value;
            // cancel lerp if its happening
            if(visualLerpCoroutine != null)
                StopCoroutine(visualLerpCoroutine);
        }
    }
    public int X
    {
        get => pos.x;
        set
        {
            Pos = new Vector2Int(value, pos.y);
        }
    }
    public int Y
    {
        get => pos.y;
        set
        {
            Pos = new Vector2Int(pos.x, value);
        }
    }

    public void SmoothMove(Vector2Int newPos)
    {
        pos = newPos;
        if (visualLerpCoroutine != null)
            StopCoroutine(visualLerpCoroutine);
        visualLerpCoroutine = StartCoroutine(LerpTo(newPos));
    }

    private IEnumerator LerpTo(Vector2Int destination)
    {
        Vector2 start = transform.position;
        Vector2 end = (Vector2)destination;

        float elapsed = 0.0f;
        float time = 0.1f;
        while(elapsed < time)
        {
            float t = elapsed / time;
            transform.position = Vector2.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    private void Awake()
    {
        // NOTE: Start() will affect all components when they are created
        // Awake() only works on scene start, which is why I use it

        // copy transform.position into this componenet
        pos = Vector2Int.RoundToInt(transform.position);
    }

    private void OnDestroy()
    {
        // hide the gameobject offscreen
        transform.position = (Vector2)Constants.OFFSCREEN;
    }
}
