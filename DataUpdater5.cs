using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

public class DataUpdater5
{
    public static void UpdateValue(string filePath, string propertyPath, string newValue)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            UpdateJsonValue(filePath, propertyPath, newValue);
        }
        else if (extension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
        {
            UpdateXmlValue(filePath, propertyPath, newValue);
        }
        else
        {
            throw new NotSupportedException("File format not supported. Only JSON and XML are supported.");
        }
    }

    private static void UpdateJsonValue(string filePath, string propertyPath, string newValue)
    {
        var json = File.ReadAllText(filePath);
        var jsonObject = JObject.Parse(json);

        if (propertyPath.Contains(":"))
        {
            UpdateSpecificProperty(jsonObject, propertyPath.Split(':'), newValue);
        }
        else
        {
            UpdateRootProperty(jsonObject, propertyPath, newValue);
        }

        File.WriteAllText(filePath, jsonObject.ToString());
    }

    private static void UpdateXmlValue(string filePath, string propertyPath, string newValue)
    {
        var xml = File.ReadAllText(filePath);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        string json = JsonConvert.SerializeXmlNode(xmlDoc);
        var jsonObject = JObject.Parse(json);

        if (propertyPath.Contains(":"))
        {
            UpdateSpecificProperty(jsonObject, propertyPath.Split(':'), newValue);
        }
        else
        {
            UpdateRootProperty(jsonObject, propertyPath, newValue);
        }

        string updatedJson = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
        xmlDoc = JsonConvert.DeserializeXmlNode(updatedJson);
        xmlDoc?.Save(filePath);
    }

    private static void UpdateRootProperty(JToken token, string property, string newValue)
    {
        if (token.Type == JTokenType.Object)
        {
            var prop = token[property];
            if (prop != null)
            {
                prop.Replace(newValue);
            }
            else
            {
                ((JObject)token).Add(property, newValue);
            }
        }
    }

    private static void UpdateSpecificProperty(JToken token, string[] path, string newValue, int index = 0)
    {
        if (index >= path.Length) return;

        string currentKey = path[index];

        if (token.Type == JTokenType.Object)
        {
            var obj = (JObject)token;
            if (!obj.ContainsKey(currentKey))
            {
                // Create array or object based on next key
                bool nextIsIndex = index + 1 < path.Length && int.TryParse(path[index + 1], out _);
                obj[currentKey] = nextIsIndex ? new JArray() : new JObject();
            }

            UpdateSpecificProperty(obj[currentKey], path, newValue, index + 1);
        }
        else if (token.Type == JTokenType.Array)
        {
            var array = (JArray)token;

            if (int.TryParse(currentKey, out int arrayIndex))
            {
                while (array.Count <= arrayIndex)
                {
                    array.Add(JValue.CreateNull());
                }

                if (index == path.Length - 1)
                {
                    array[arrayIndex] = newValue; // ✅ Replace value at index
                }
                else
                {
                    if (array[arrayIndex] == null || array[arrayIndex].Type != JTokenType.Object)
                    {
                        array[arrayIndex] = new JObject();
                    }

                    UpdateSpecificProperty(array[arrayIndex], path, newValue, index + 1);
                }
            }
        }
    }







}
