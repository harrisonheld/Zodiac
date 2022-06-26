using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity with this component is one that physically exists.
/// </summary>
public class Physical : MonoBehaviour
{
    // some far away point to put stuff
    public static Vector2Int OFFSCREEN = new Vector2Int(-100, -100);

    private Vector2Int position;
    public Vector2Int Position { 
        get => position;
        set {
            position = value;
            // update transform position
            this.gameObject.transform.position = (Vector2)value;
        }
    }
    [SerializeField] public bool Solid = false; // can other entities walk through this?
    [SerializeField] public bool OccludesVison = false; // should this block line of sight?

    private void Start()
    {
        // copy transform.position into this componenet
        Position = Vector2Int.RoundToInt(transform.position);
    }

    private void OnDestroy()
    {
        // hide the gameobject offscreen
        Position = OFFSCREEN;
    }
}
