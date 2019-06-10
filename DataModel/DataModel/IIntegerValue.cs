using System;

namespace DistributedExperimentation.DataModel
{
    public interface IIntegerValue : IPrimitiveValue
    {
        ValueType getIntegerValue();
    }
}