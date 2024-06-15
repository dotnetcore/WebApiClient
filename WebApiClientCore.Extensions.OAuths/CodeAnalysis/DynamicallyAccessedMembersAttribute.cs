#if NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that the members accessed dynamically at runtime are considered used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, Inherited = false)]
    sealed class DynamicallyAccessedMembersAttribute : Attribute
    {
        /// <summary>
        /// Gets the types of dynamically accessed members.
        /// </summary>
        public DynamicallyAccessedMemberTypes MemberTypes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicallyAccessedMembersAttribute"/> class with the specified member types.
        /// </summary>
        /// <param name="memberTypes">The types of dynamically accessed members.</param>
        public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
        {
            this.MemberTypes = memberTypes;
        }
    }
}
#endif