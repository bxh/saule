using System;
using System.Linq;
using Saule.Serialization;

namespace Saule
{
    public abstract partial class ApiResource : IDeserializationPropertyMap, ISerializationPropertyMap
    {
        string IDeserializationPropertyMap.GetIdPropertyName(
            string resourceType,
            Type resourceObjectType)
        {
            if (resourceType == ResourceType)
            {
                return IdProperty;
            }

            return Relationships
                    .FirstOrDefault(r => r.RelatedResource.ResourceType == resourceType)?
                    .RelatedResource.IdProperty;
        }

        string IDeserializationPropertyMap.GetAttributePropertyName(
            string resourceType,
            string attributeName,
            Type resourceObjectType)
        {
            if (Attributes.Any(a => a.Name == attributeName))
            {
                return attributeName.ToPascalCase();
            }

            throw new ArgumentException($"No such attribute ({attributeName}).");
        }

        string IDeserializationPropertyMap.GetRelationshipPropertyName(
            string resourceType,
            string relationshipName,
            Type resourceObjectType,
            string relationshipResourceType)
        {
            if (Relationships.Any(a => a.Name == relationshipName))
            {
                return relationshipName.ToPascalCase();
            }

            throw new ArgumentException($"No such relationship ({relationshipName}).");
        }

        string ISerializationPropertyMap.GetResourceTypeName(Type resourceObjectType)
        {
            return ResourceType;
        }

        PropertyType ISerializationPropertyMap.GetPropertyType(Type resourceObjectType, string resourceType, string propertyName)
        {
            var dashed = propertyName.ToDashed();
            return Attributes.Any(a => a.Name == dashed)
                ? PropertyType.Attribute
                : Relationships.Any(r => r.Name == dashed)
                    ? PropertyType.Relationship
                    : PropertyType.NotUsed;
        }

        string ISerializationPropertyMap.GetAttributeName(Type resourceObjectType, string resourceType, string propertyName)
        {
            if ((this as ISerializationPropertyMap).GetPropertyType(resourceObjectType, resourceType, propertyName) !=
                PropertyType.Attribute)
            {
                throw new ArgumentException($"Property {propertyName} is not an attribute on {ResourceType}.");
            }

            return propertyName.ToDashed();
        }

        string ISerializationPropertyMap.GetRelationshipName(Type resourceObjectType, string resourceType, string propertyName)
        {
            if ((this as ISerializationPropertyMap).GetPropertyType(resourceObjectType, resourceType, propertyName) !=
                PropertyType.Relationship)
            {
                throw new ArgumentException($"Property {propertyName} is not a relationship on {ResourceType}.");
            }

            return propertyName.ToDashed();
        }
    }
}
