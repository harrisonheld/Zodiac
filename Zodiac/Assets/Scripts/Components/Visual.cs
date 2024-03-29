﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            displayName = value;
            gameObject.name = value;
        }
    }
    [field: SerializeField] public string Description { get; set; } = "DESCRIPTION_HERE";
    [field: SerializeField] private string spriteName = "";

    private Material material;
    private Color colorPrimary = Color.white;
    private Color colorSecondary = Color.gray;
    private Color colorTertiary = Color.black;
    public Color ColorPrimary {
        get => colorPrimary;
        set
        {
            colorPrimary = value;
            if(material != null)
                material.SetColor("_Out1", colorPrimary);
        }
    }
    public Color ColorSecondary
    {
        get => colorSecondary;
        set
        {
            colorSecondary = value;
            if (material != null)
                material.SetColor("_Out2", colorSecondary);
        }
    }
    public Color ColorTertiary
    {
        get => colorTertiary;
        set
        {
            colorTertiary = value;
            if (material != null)
                material.SetColor("_Out3", colorTertiary);
        }
    }

    /// <summary>The sprite as a string.</summary>
    public string Sprite
    {
        get
        {
            return spriteName;
        }
        set
        {
            Sprite loaded = Resources.Load<Sprite>("Sprites/" + value);
            if(loaded == null)
            {
                Debug.LogWarning("Sprite not found: " + value);
                this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Misc/error");
                return;
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = loaded;
            spriteName = value;
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(DisplayName);
        writer.Write(Description);
        writer.Write(Sprite);
        WriteColor(writer, colorPrimary);
        WriteColor(writer, colorSecondary);
        WriteColor(writer, colorTertiary);
    }
    public override void Deserialize(BinaryReader reader, Dictionary<int, GameObject> idToEntity = null)
    {
        DisplayName = reader.ReadString();
        Description = reader.ReadString();
        Sprite = reader.ReadString();
        ColorPrimary = ReadColor(reader);
        ColorSecondary = ReadColor(reader);
        ColorTertiary = ReadColor(reader);
    }

    public Sprite GetUnitySprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    private void Awake()
    {
        // add sprite renderer if necessary
        if (gameObject.GetComponent<SpriteRenderer>() == null)
            gameObject.AddComponent<SpriteRenderer>();
        
        // grab values from unity
        if (DisplayName == "")
            DisplayName = gameObject.name;
        if (spriteName == "")
            spriteName = gameObject.GetComponent<SpriteRenderer>().sprite?.name;

        // add shader
        material = new Material(Shader.Find("Unlit/PaletteSwap"));
        material.SetColor("_Out1", colorPrimary);
        material.SetColor("_Out2", colorSecondary);
        material.SetColor("_Out3", colorTertiary);

        // assign the material to the renderer
        GetComponent<SpriteRenderer>().material = material;
    }
}