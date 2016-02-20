using System;

namespace Saule.Serialization
{
    internal interface IResourceDeserializationPropertyMap
    {
        string GetIdPropertyName(
            string resourceType,
            Type resourceObjectType);

        string GetAttributePropertyName(
            string resourceType,
            string attributeName,
            Type resourceObjectType);

        string GetRelationshipPropertyName(
            string resourceType,
            string relationshipName,
            Type resourceObjectType,
            string relationshipResourceType);
    }
}