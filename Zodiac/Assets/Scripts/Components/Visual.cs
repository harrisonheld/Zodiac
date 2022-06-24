using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visual : MonoBehaviour
{
    [SerializeField] public string Name = "";
    [SerializeField] private Sprite sprite = null;
    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
        set
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = value;
            sprite = value;
        }
    }

    private void Start()
    {
        // grab values from unity

        if(Name == "")
            Name = gameObject.name;

        if (sprite == null)
            Sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }
}