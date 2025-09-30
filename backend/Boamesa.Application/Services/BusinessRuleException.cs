// Boamesa.Application/Services/BusinessRuleException.cs
using System.Runtime.Serialization;

namespace Boamesa.Application.Services;

[Serializable]
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) {}
    protected BusinessRuleException(SerializationInfo info, StreamingContext context) : base(info, context) {}
}