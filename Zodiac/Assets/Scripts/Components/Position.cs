using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Position : MonoBehaviour
{
    // some far away point to put stuff
    public static Vector2Int OFFSCREEN = new Vector2Int(-100, -100);

    private Vector2Int pos;
    public Vector2Int Pos
    {
        get => pos;
        set
        {
            pos = value;
            // update transform position
            this.gameObject.transform.position = (Vector2)value;
        }
    }

    private void Start()
    {
        // copy transform.position into this componenet
        Pos = Vector2Int.RoundToInt(transform.position);
    }

    private void OnDestroy()
    {
        // hide the gameobject offscreen
        Pos = OFFSCREEN;
    }
}
