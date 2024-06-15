#if NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies the types of dynamically accessed members.
    /// </summary>
    enum DynamicallyAccessedMemberTypes
    {
        /// <summary>
        /// All member types are dynamically accessed.
        /// </summary>
        All = -1,

        /// <summary>
        /// No member types are dynamically accessed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Public parameterless constructors are dynamically accessed.
        /// </summary>
        PublicParameterlessConstructor = 1,

        /// <summary>
        /// Public constructors are dynamically accessed.
        /// </summary>
        PublicConstructors = 3,

        /// <summary>
        /// Non-public constructors are dynamically accessed.
        /// </summary>
        NonPublicConstructors = 4,

        /// <summary>
        /// Public methods are dynamically accessed.
        /// </summary>
        PublicMethods = 8,

        /// <summary>
        /// Non-public methods are dynamically accessed.
        /// </summary>
        NonPublicMethods = 16,

        /// <summary>
        /// Public fields are dynamically accessed.
        /// </summary>
        PublicFields = 32,

        /// <summary>
        /// Non-public fields are dynamically accessed.
        /// </summary>
        NonPublicFields = 64,

        /// <summary>
        /// Public nested types are dynamically accessed.
        /// </summary>
        PublicNestedTypes = 128,

        /// <summary>
        /// Non-public nested types are dynamically accessed.
        /// </summary>
        NonPublicNestedTypes = 256,

        /// <summary>
        /// Public properties are dynamically accessed.
        /// </summary>
        PublicProperties = 512,

        /// <summary>
        /// Non-public properties are dynamically accessed.
        /// </summary>
        NonPublicProperties = 1024,

        /// <summary>
        /// Public events are dynamically accessed.
        /// </summary>
        PublicEvents = 2048,

        /// <summary>
        /// Non-public events are dynamically accessed.
        /// </summary>
        NonPublicEvents = 4096,
    }
}
#endif