using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using Newtonsoft.Json.Converters;

namespace UpdateJsonValue
{
    public static class DataUpdater
    {
        public static void UpdateValue(string filePath, string propertyPath, string newValue)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension == ".json")
            {
                UpdateJsonValue(filePath, propertyPath, newValue);
            }
            else if (extension == ".xml")
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
                UpdateAllProperties(jsonObject, propertyPath, newValue);
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
                UpdateAllProperties(jsonObject, propertyPath, newValue);
            }

            string updatedJson = jsonObject.ToString();
            XmlDocument updatedXmlDoc = JsonConvert.DeserializeXmlNode(updatedJson);
            File.WriteAllText(filePath, updatedXmlDoc.OuterXml);
        }

        private static void UpdateAllProperties(JToken token, string property, string newValue)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var prop in token.Children<JProperty>())
                {
                    if (prop.Name == property)
                    {
                        prop.Value = newValue;
                    }
                    else
                    {
                        UpdateAllProperties(prop.Value, property, newValue);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    UpdateAllProperties(item, property, newValue);
                }
            }
        }

        private static void UpdateSpecificProperty(JToken token, string[] path, string newValue, int index = 0)
        {
            if (index >= path.Length) return;

            if (token.Type == JTokenType.Object)
            {
                var prop = token[path[index]];
                if (prop != null)
                {
                    if (index == path.Length - 1)
                    {
                        prop.Replace(newValue);
                    }
                    else
                    {
                        UpdateSpecificProperty(prop, path, newValue, index + 1);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    UpdateSpecificProperty(item, path, newValue, index);
                }
            }
        }
    }
}
