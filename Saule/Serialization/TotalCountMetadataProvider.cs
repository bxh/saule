using System.Collections.Generic;
using System.Linq;

namespace Saule.Serialization
{
    internal sealed class TotalCountMetadataProvider<T> : IMetadataProvider<IEnumerable<T>, CountMetadata>
    {
        public CountMetadata BuildMetadata(IEnumerable<T> value)
        {
            return new CountMetadata { ItemCount = value.Count() };
        }
    }
}