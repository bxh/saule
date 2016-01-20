namespace Saule.Serialization
{
    /// <summary>
    /// Implementations of this interface are used to provide metadata
    /// about the response to the serialization process.
    /// </summary>
    /// <typeparam name="TResponse">The type of the value returned by the action method.</typeparam>
    /// <typeparam name="TMetadata">The type of the metadata object.</typeparam>
    public interface IMetadataProvider<in TResponse, out TMetadata>
        where TMetadata : ResponseMetadata
    {
        /// <summary>
        /// Builds metadata for the returned value.
        /// </summary>
        /// <param name="value">The response to provide metadata for.</param>
        /// <returns>The metadata for the given response.</returns>
        TMetadata BuildMetadata(TResponse value);
    }
}
