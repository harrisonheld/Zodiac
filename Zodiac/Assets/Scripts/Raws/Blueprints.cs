using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Raws
{
    public static class Blueprints
    {
        private const string BLUEPRINTS_DIR = @"C:\Users\johnd\Unity Projects\ZodiacRepo\Zodiac\Assets\Resources\Raws\Blueprints\";

        private static Dictionary<string, EntityBlueprint> _blueprints = new();
        private static bool _initialized = false;
        public static void Initialize()
        {
            _blueprints.Clear();

            foreach (string file in Directory.GetFiles(BLUEPRINTS_DIR))
            {
                if (!file.EndsWith(".json"))
                    continue;

                string fullPath = Path.Combine(BLUEPRINTS_DIR, file);

                string json = File.ReadAllText(fullPath);
                List<EntityBlueprint> blueprintsInFile = JsonConvert.DeserializeObject<List<EntityBlueprint>>(json);

                foreach (EntityBlueprint entityBlueprint in blueprintsInFile)
                {
                    _blueprints.Add(entityBlueprint.Id, entityBlueprint);
                }
            }

            _initialized = true;
        }

        public static GameObject FromBlueprint(string blueprintId)
        {
            if (!_initialized)
                Initialize();

            EntityBlueprint blueprint = _blueprints[blueprintId];
            GameObject entity = null;
            if (string.IsNullOrEmpty(blueprint.Inherits))
            {
                entity = new GameObject();
            }
            else
            {
                entity = FromBlueprint(blueprint.Inherits);
            }

            // add components
            foreach (ComponentBlueprint componentBlueprint in blueprint.ComponentBlueprints)
            {
                Type componentType = Type.GetType(componentBlueprint.Type);
                if (componentType == null)
                {
                    string errorMsg = $"Type '{componentBlueprint.Type}' does not exist.";
                    throw new Exception(errorMsg);
                }
                PropertyInfo[] componentPropertyInfos = componentType.GetProperties();

                // get or create a ZodiacComponent to write properties into
                ZodiacComponent zodiacComponent = null;
                // if the base object has a DisallowMultiple type of component...
                Component preexistingComponent = entity.GetComponent(componentType);
                if (Attribute.IsDefined(componentType, typeof(DisallowMultipleComponent)) && preexistingComponent != null)
                {
                    // ...we will write updated properties to that
                    zodiacComponent = preexistingComponent as ZodiacComponent;
                }
                else
                {
                    // ...otherwise we will make a new component to write to
                    zodiacComponent = entity.AddComponent(componentType) as ZodiacComponent;
                }

                foreach (KeyValuePair<string, object> property in componentBlueprint.Properties)
                {
                    string propName = property.Key;
                    object propVal = property.Value;

                    PropertyInfo propInfo = componentType.GetProperty(propName);
                    if (propInfo == null)
                    {
                        string errorMsg = $"Type '{componentType.FullName}' has no property '{propName}'.";
                        throw new Exception(errorMsg);
                    }
                    Type propType = propInfo.PropertyType;

                    // if the type is a kind of list
                    if (typeof(IList).IsAssignableFrom(propType))
                    {
                        // TODO
                    }
                    // if the type is a dictionary
                    else if(propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        // TODO
                        continue;
                    }
                    else if (propType.IsEnum)
                    {
                        propVal = Enum.Parse(propType, propVal as string);
                        propInfo.SetValue(zodiacComponent, propVal);
                    }
                    else if (propType == typeof(Color))
                    {
                        // seperate string into rgb components (these are [0.0, 1.0], not [0,255])
                        float[] rgb = (propVal as string).Split(',').Select(component => float.Parse(component)).ToArray();
                        propInfo.SetValue(zodiacComponent, new Color(rgb[0], rgb[1], rgb[2]));
                    }
                    else
                    {
                        propVal = System.Convert.ChangeType(propVal, propType);
                        propInfo.SetValue(zodiacComponent, propVal);
                    }
                }
            }

            return entity;
        }
        public static GameObject FromBlueprint(string blueprintId, Vector2Int pos)
        {
            GameObject result = FromBlueprint(blueprintId);
            result.AddComponent<Position>();
            result.GetComponent<Position>().Pos = pos;
            GameManager.Instance.Entities.Add(result);
            return result;
        }

        private class EntityBlueprint
        {
            public string Id { get; set; } = null;
            public string Inherits { get; set; } = null;
            [JsonProperty("Components")]
            public List<ComponentBlueprint> ComponentBlueprints { get; set; } = new();
        }

        private class ComponentBlueprint
        {
            public string Type { get; set; } = null;
            public Dictionary<string, object> Properties { get; set; } = new();
        }
    }
}