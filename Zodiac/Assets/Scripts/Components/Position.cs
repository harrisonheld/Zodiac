using System;
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
            // invoke event
            PositionChanged?.Invoke(this, new PositionChangedArgs(X, Y, value.x, value.y));
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

    public event EventHandler<PositionChangedArgs> PositionChanged;
    public class PositionChangedArgs : EventArgs
    {
        public readonly int OldX;
        public readonly int OldY;
        public readonly int NewX;
        public readonly int NewY;
        
        public PositionChangedArgs(int _oldX, int _oldY, int _newX, int _newY)
        {
            OldX = _oldX;
            OldY = _oldY;
            NewX = _newX;
            NewY = _newY;
        }
    }

    public void SmoothMove(Vector2Int newPos)
    {
        PositionChanged?.Invoke(this, new PositionChangedArgs(X, Y, newPos.x, newPos.y));

        pos = newPos;
        if (visualLerpCoroutine != null)
        {
            StopCoroutine(visualLerpCoroutine);
        }
        visualLerpCoroutine = StartCoroutine(LerpTo(newPos));
    }
    public void VisualBump(Vector2Int target)
    {
        if (visualLerpCoroutine != null)
        {
            StopCoroutine(visualLerpCoroutine);
            transform.position = (Vector2)pos;
        }

        // only bump a quarters of the way
        Vector2 partial = Vector2.Lerp(pos, target, 0.25f);
        // bump to the target and back, only visually
        visualLerpCoroutine = StartCoroutine(LerpToAndBack(partial));
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
    private IEnumerator LerpToAndBack(Vector2 destination)
    {
        Vector2 start = transform.position;
        Vector2 end = destination;

        float elapsed = 0.0f;
        float time = 0.25f;

        // lerp to
        while (elapsed < time / 2)
        {
            float t = 2 * elapsed / time;
            transform.position = Vector2.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // lerp back
        while (elapsed < time)
        {
            float t = 2 * elapsed / time - 1;
            transform.position = Vector2.Lerp(end, start, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = start;
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
