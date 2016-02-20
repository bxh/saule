using System;
using System.Linq;
using System.Reflection;

namespace Saule.Serialization
{
    internal class DefaultPropertyMap : IResourceDeserializationPropertyMap
    {
        private readonly ApiResource _resource;

        public DefaultPropertyMap(ApiResource resource)
        {
            _resource = resource;
        }

        public string GetIdPropertyName(string resourceType, Type resourceObjectType)
        {
            if (resourceType == _resource.ResourceType)
            {
                return _resource.IdProperty;
            }

            var related = _resource.Relationships.FirstOrDefault(r => r.RelatedResource.ResourceType == resourceType);
            return related?.RelatedResource.IdProperty;
        }

        public string GetAttributePropertyName(string resourceType, string attributeName, Type resourceObjectType)
        {
            return attributeName.ToPascalCase();
        }

        public string GetRelationshipPropertyName(
            string resourceType,
            string relationshipName,
            Type resourceObjectType,
            string relationshipResourceType)
        {
            return relationshipName.ToPascalCase();
        }
    }
}
