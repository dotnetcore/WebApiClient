namespace WebApiClient
{
    /// <summary>
    /// Collection formatting
    /// </summary>
    public enum CollectionFormat
    {
        /// <summary>
        /// Comma separated
        /// value1,value2
        /// </summary>
        Csv,

        /// <summary>
        /// Space-separated
        /// value1 value2
        /// </summary>
        Ssv,

        /// <summary>
        /// Backslash-separated
        /// value1\value2
        /// </summary>
        Tsv,

        /// <summary>
        /// Pipe-separated
        /// value1|value2
        /// </summary>
        Pipes,

        /// <summary>
        /// Single attribute can take multiple values
        /// </summary>
        Multi
    }
}
