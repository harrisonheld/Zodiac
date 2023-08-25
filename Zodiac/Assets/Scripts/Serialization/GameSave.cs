using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;

using System.IO;
using WorldGen;
using QuestNamespace;

namespace Zodiac.Serialization
{
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

        public void SaveExtraData()
        {
            string path = Path.Combine(FolderName, "save.star");
            using FileStream stream = new FileStream(path, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);

            QuestManager.Instance.Serialize(writer);
        }
        public void LoadExtraData()
        {
            string path = Path.Combine(FolderName, "save.star");
            using FileStream stream = new FileStream(path, FileMode.Open);
            using BinaryReader reader = new BinaryReader(stream);

            QuestManager.Instance.Deserialize(reader);
        }
        public void SaveZone(ZoneInfo info)
        {
            SaveExtraData(); // save the whole game

            string path = ZoneFilePath(info.X, info.Y);
            using FileStream stream = new FileStream(path, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);
            
            info.Serialize(writer);

            ZoneContentSerializer serializer = new();
            serializer.SerializeScene(writer);
        }
        public ZoneInfo LoadZone(int x, int y)
        {
            LoadExtraData(); // load the whole game

            string path = ZoneFilePath(x, y);
            using FileStream stream = new FileStream(path, FileMode.Open);
            using BinaryReader reader = new BinaryReader(stream);

            ZoneInfo info = new();
            info.Deserialize(reader);

            ZoneContentDeserializer deserializer = new();
            List<GameObject> entities = deserializer.DeserializeScene(reader);

            IEnumerable<GameObject> havePos = entities.Where(e => e.GetComponent<Position>() != null);
            GameManager.Instance.Entities.AddRange(havePos);

            return info;
        }
        public bool isZoneSaved(int x, int y)
        {
            string path = ZoneFilePath(x, y);
            return File.Exists(path);
        }
        private string ZoneFilePath(int x, int y)
        {
            string path = Path.Combine(FolderName, $"screen{x}-{y}.star");
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

}
