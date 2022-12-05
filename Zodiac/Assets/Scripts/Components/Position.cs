using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Position : ZodiacComponent
{
    [SerializeField] private Vector2Int pos;
    public Vector2Int Pos
    {
        get => pos;
        set
        {
            pos = value;
            // update transform position
            gameObject.transform.position = (Vector2)value;
        }
    }

    public void SmoothMove(Vector2Int newPos)
    {
        pos = newPos;
        StartCoroutine(LerpTo(newPos));
    }

    private IEnumerator LerpTo(Vector2Int destination)
    {
        Debug.Log(gameObject.name + " " + transform.position + " " + destination);

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
