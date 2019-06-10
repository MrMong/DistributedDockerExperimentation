using System;

namespace DistributedExperimentation.DataModel
{
    public interface IPrimitiveValue : IParameterValue
    {
        bool isInteger();
        bool isReal();
        bool isBoolean();
        bool isCharacterString();
        String getRawValue();
    }
}