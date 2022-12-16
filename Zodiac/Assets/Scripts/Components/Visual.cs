using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Visual : ZodiacComponent
{
    [SerializeField]
    private string displayName = "DISPLAY_NAME_HERE";
    public string DisplayName
    {
        get
        {
            return displayName;
        }
        set
        {
            this.gameObject.name = value;
            displayName = value;
        }
    }
    [field: SerializeField] public string Description { get; set; } = "DESCRIPTION_HERE";
    [field: SerializeField] private string spriteName = "";

    /// <summary>
    /// The sprite as a string.
    /// </summary>
    public string Sprite
    {
        get
        {
            return spriteName;
        }
        set
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + value);
            spriteName = value;
        }
    }

    public Sprite GetUnitySprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    private void Awake()
    {
        // grab values from unity
        if(DisplayName == "")
            DisplayName = gameObject.name;
        if (spriteName == "")
            spriteName = gameObject.GetComponent<SpriteRenderer>().sprite?.name;

        // add shader
        Material material = new Material(Shader.Find("Unlit/PaletteSwap"));
        material.SetColor("_Out1", Color.white);
        material.SetColor("_Out2", Color.grey);
        material.SetColor("_Out3", Color.black);

        // assign the material to the renderer
        GetComponent<SpriteRenderer>().material = material;
    }
}