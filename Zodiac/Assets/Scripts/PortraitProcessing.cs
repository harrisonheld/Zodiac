using UnityEditor;
using UnityEngine;

public class PortraitProcessing : MonoBehaviour
{
    // The folder containing the original portraits
    public string originalPortraitsFolderPath = "Assets/Textures/UnfilteredPortraits/";
    // The folder to save the filtered portraits
    public string filteredPortraitsFolderPath = "Assets/Resources/Portraits/";

    public int outputWidth = 128;
    public int outputHeight = 128;

    // Function to apply the filter and save the filtered portraits
    public void PreprocessAllPortraits()
    {
        // Get all the texture files from the original portraits folder
        string[] originalPortraitFiles = System.IO.Directory.GetFiles(originalPortraitsFolderPath, "*.png");

        foreach (string filePath in originalPortraitFiles)
        {
            // Load the original portrait texture
            Texture2D originalPortrait = LoadTextureFromFile(filePath);

            // scale portrait
            int widthScaleFactor = originalPortrait.width / outputWidth;
            int heightScaleFactor = originalPortrait.height / outputHeight;
            Texture2D filteredPortrait = new Texture2D(outputWidth, outputHeight, TextureFormat.ARGB32, false);
            filteredPortrait.filterMode = FilterMode.Point; // for pixelated look
            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    int originalX = Mathf.RoundToInt(x * widthScaleFactor);
                    int originalY = Mathf.RoundToInt(y * heightScaleFactor);

                    Color color = originalPortrait.GetPixel(originalX, originalY);

                    filteredPortrait.SetPixel(x, y, color);
                }
            }

            // apply 4tone monochrome
            Color[] pixels = filteredPortrait.GetPixels();
            Color[] newPixels = new Color[pixels.Length];

            Color black = new Color(0, 0, 0);
            Color darkGray = new Color(0.25f, 0.25f, 0.25f);
            Color lightGray = new Color(0.75f, 0.75f, 0.75f);
            Color white = new Color(1, 1, 1);

            float threshold = 0.25f;

            for (int i = 0; i < pixels.Length; i++)
            {
                float grayscale = pixels[i].grayscale;

                if (grayscale < threshold)
                    newPixels[i] = black;
                else if (grayscale < 2 * threshold)
                    newPixels[i] = darkGray;
                else if (grayscale < 3 * threshold)
                    newPixels[i] = lightGray;
                else
                    newPixels[i] = white;
            }

            filteredPortrait.SetPixels(newPixels);
            filteredPortrait.Apply();

            // Save the filtered portrait as a PNG file in the filtered portraits folder
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string outputPath = System.IO.Path.Combine(filteredPortraitsFolderPath, $"{fileName}.png");
            byte[] bytes = filteredPortrait.EncodeToPNG();
            System.IO.File.WriteAllBytes(outputPath, bytes);
        }
    }

    // Function to load a texture from a file
    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2); // Provide initial dimensions
        texture.LoadImage(fileData);
        return texture;
    }
}


[CustomEditor(typeof(PortraitProcessing))]
public class PortraitPreprocessingEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Preprocess Portraits"))
        {
            PortraitProcessing portraitProcessing = (PortraitProcessing)target;
            portraitProcessing.PreprocessAllPortraits();
        }
    }
}