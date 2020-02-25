namespace WebApiClient
{
    /// <summary>
    /// Defines whether the feature allows duplicate declarations on interfaces and methods
    /// If not allowed, prefer methodological features
    /// </summary>
    public interface IAttributeMultiplable
    {
        /// <summary>
        /// Obtaining a Sorted Index
        /// </summary>
        int OrderIndex { get; }

        /// <summary>
        /// Gets whether this type allows duplicates in interfaces and methods
        /// </summary>
        bool AllowMultiple { get; }
    }
}
