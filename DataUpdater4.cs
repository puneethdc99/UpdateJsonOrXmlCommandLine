namespace UpdateJsonValue
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Xml;

    public class DataUpdater4
    {
        public static void UpdateValue(string filePath, string propertyPath, string newValue)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                UpdateJsonValue(filePath, propertyPath, newValue);
            }
            else if (extension.Equals(".xml",StringComparison.OrdinalIgnoreCase))
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
            XmlDocument? xmlDoc = new XmlDocument();
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

            //string updatedJson = jsonObject.ToString();
            //XmlDocument updatedXmlDoc = JsonConvert.DeserializeXmlNode(updatedJson);
            //File.WriteAllText(filePath, updatedXmlDoc.OuterXml);
            string updatedJson = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
            xmlDoc = JsonConvert.DeserializeXmlNode(updatedJson);
            //File.WriteAllText(filePath, updatedXmlDoc.OuterXml);
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
                else
                {
                    if (index == path.Length - 1)
                    {
                        ((JObject)token).Add(path[index], newValue);
                    }
                    else
                    {
                        var newObject = new JObject();
                        ((JObject)token).Add(path[index], newObject);
                        UpdateSpecificProperty(newObject, path, newValue, index + 1);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                if (int.TryParse(path[index], out int arrayIndex) && arrayIndex < token.Count())
                {
                    var item = token[arrayIndex];
                    if (index == path.Length - 1)
                    {
                        item?.Replace(newValue);
                    }
                    else
                    {
                        UpdateSpecificProperty(item, path, newValue, index + 1);
                    }
                }
                else
                {
                    if (index == path.Length - 1)
                    {
                        ((JArray)token).Add(newValue);
                    }
                    else
                    {
                        var newObject = new JObject();
                        ((JArray)token).Add(newObject);
                        UpdateSpecificProperty(newObject, path, newValue, index + 1);
                    }
                }
            }
        }
    }
}
