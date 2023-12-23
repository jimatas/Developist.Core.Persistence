namespace System.Diagnostics.CodeAnalysis;

#if !NETCOREAPP3_0_OR_GREATER
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
internal sealed class NotNullAttribute : Attribute
{
}
#endif
