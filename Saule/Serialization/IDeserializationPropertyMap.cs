using System;

namespace Saule.Serialization
{
    internal interface IDeserializationPropertyMap
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

    internal interface ISerializationPropertyMap
    {
        string GetResourceTypeName(
            Type resourceObjectType);

        PropertyType GetPropertyType(
            Type resourceObjectType,
            string resourceType,
            string propertyName);

        string GetAttributeName(
            Type resourceObjectType,
            string resourceType,
            string propertyName);

        string GetRelationshipName(
            Type resourceObjectType,
            string resourceType,
            string propertyName);
    }

    internal enum PropertyType
    {
        /// <summary>
        /// Indicates that the property should not be serialized.
        /// </summary>
        NotUsed,

        /// <summary>
        /// Indicates that the property should be serialized as an attribute.
        /// </summary>
        Attribute,

        /// <summary>
        /// Indicates that the property should be serialized as a relationship.
        /// </summary>
        Relationship
    }
}