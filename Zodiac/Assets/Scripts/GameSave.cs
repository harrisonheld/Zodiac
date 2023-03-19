using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;

using System.IO;

class GameSave
{
    public string FolderName { get; private set; }
    public readonly int WorldSeed;

    public GameSave()
    {
        // find a folder name that doesn't exist yet
        int i = 1;
        do
        {
            FolderName = Path.Combine(Application.persistentDataPath, $"Save{i}");
            i++;
        } while (Directory.Exists(FolderName));
        
        // create it yay
        Directory.CreateDirectory(FolderName);

        // le world seed
        WorldSeed = 69;
    }
    
    public void SaveScreen(int x, int y)
    {
        EntitySerializer serializer = new();
        string path = ScreenFilePath(x, y);
        serializer.SerializeScene(path);
    }
    public List<GameObject> LoadScreen(int x, int y)
    {
        string path = ScreenFilePath(x, y);
        EntitySerializer serializer = new();
        return serializer.DeserializeScene(path);
    }
    public bool ScreenSaved(int x, int y)
    {
        string path = ScreenFilePath(x, y);
        return File.Exists(path);
    }
    private string ScreenFilePath(int x, int y)
    {
        string path = Path.Combine(FolderName, $"screen{x}-{y}.xml");
        return path;
    }

    public long FileSizeBytes()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(FolderName);
        return dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
    }
    public float FileSizeKilobytes()
    {
        return FileSizeBytes() / 1000f;
    }
}
