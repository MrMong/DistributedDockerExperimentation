using System;

namespace DistributedExperimentation.DataModel
{
    public interface IBooleanValue : IPrimitiveValue
    {
        ValueType getBooleanValue();
    }
}