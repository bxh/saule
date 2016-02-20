using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Saule.Serialization
{
    internal class ResourceDeserializer
    {
        private readonly JToken _object;
        private readonly IDeserializationPropertyMap _deserializationMap;
        private Type _target;

        public ResourceDeserializer(
            JToken @object,
            Type target,
            IDeserializationPropertyMap deserializationMap)
        {
            _object = @object;
            _target = target;
            _deserializationMap = deserializationMap;
        }

        public object Deserialize()
        {
            // assume object is not an array
            var array = _object["data"] as JArray;

            if (array == null)
            {
                return DeserializeSingle(_object["data"]);
            }

            _target = _target.GetInterface("IEnumerable`1").GetGenericArguments().First();
            return array.Select(DeserializeSingle);
        }

        private static string GetRelationshipType(JProperty r)
        {
            var data = r.Value["data"];
            var array = data as JArray;

            if (array == null)
            {
                return data["type"].Value<string>();
            }

            return array.Count != 0
                ? data[0]["type"].Value<string>()
                : null;
        }

        private object DeserializeSingle(JToken json)
        {
            var flatJson = new JObject();

            var type = json["type"].Value<string>();

            var attributes = (JObject)json["attributes"];
            var relationships = (JObject)json["relationships"];

            var idProperty = new Property
            {
                Name = _deserializationMap.GetIdPropertyName(type, _target),
                Value = json["id"]
            };

            var attributeProperties = attributes.Properties()
                .Select(a => new Property
                {
                    Name = _deserializationMap.GetAttributePropertyName(type, a.Name, _target),
                    Value = a.Value
                });
            var relationshipProperties = relationships.Properties()
                .Select(r => new { Type = GetRelationshipType(r), JProp = r })
                .Select(r => new Property
                {
                    Name = _deserializationMap.GetRelationshipPropertyName(
                        type, r.JProp.Name, _target, r.Type),
                    Value = r.JProp.Value["data"],
                    Type = r.Type
                });

            SetId(flatJson, idProperty);
            SetAttributes(flatJson, attributeProperties);
            SetRelationships(flatJson, relationshipProperties);

            return flatJson.ToObject(_target);
        }

#pragma warning disable SA1204 // Static elements must appear before instance elements
        private static void SetId(JObject flatJson, Property idProperty)
        {
            flatJson[idProperty.Name] = idProperty.Value;
        }

        private static void SetAttributes(JObject flatJson, IEnumerable<Property> attributeProperties)
        {
            foreach (var property in attributeProperties)
            {
                flatJson[property.Name] = property.Value;
            }
        }
#pragma warning restore SA1204

        private void SetRelationships(JObject flatJson, IEnumerable<Property> relationshipProperties)
        {
            foreach (var property in relationshipProperties)
            {
                var info = _target.GetProperty(property.Name);

                if (info.PropertyType.IsComplexType())
                {
                    var relationshipName = _deserializationMap.GetIdPropertyName(
                        property.Type, info.PropertyType);

                    if (property.Value is JArray)
                    {
                        flatJson[property.Name] = new JArray(property.Value.Select(v => new JObject
                        {
                            [relationshipName] = v["id"]
                        }));
                    }
                    else
                    {
                        flatJson[property.Name] = new JObject
                        {
                            [relationshipName] = property.Value["id"]
                        };
                    }
                }
                else
                {
                    if (property.Value is JArray)
                    {
                        flatJson[property.Name] = new JArray(property.Value.Select(v => v["id"]));
                    }
                    else
                    {
                        flatJson[property.Name] = property.Value["id"];
                    }
                }
            }
        }

        private class Property
        {
            public string Name { get; set; }

            public JToken Value { get; set; }

            public string Type { get; set; } // can be null
        }
    }
}