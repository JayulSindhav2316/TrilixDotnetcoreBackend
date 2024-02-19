using System;


namespace Max.Data.Audit
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AuditableAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class NotAuditableAttribute : Attribute
    { }
}
