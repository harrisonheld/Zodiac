using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Visual : ZodiacComponent
{
    [SerializeField] public string DisplayName = "DISPLAY_NAME_HERE";
    [SerializeField] public string Description = "DESCRIPTION_HERE";
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

    private void Awake()
    {
        // grab values from unity
        if(DisplayName == "")
            DisplayName = gameObject.name;
        if (sprite == null)
            Sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        // add shader
        Material material = new Material(Shader.Find("Unlit/PaletteSwap"));
        material.SetColor("_Out1", Color.white);
        material.SetColor("_Out2", Color.grey);
        material.SetColor("_Out3", Color.black);

        // assign the material to the renderer
        GetComponent<SpriteRenderer>().material = material;
    }
}