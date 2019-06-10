using System;

namespace DistributedExperimentation.DataModel
{
    public interface IRealValue : IPrimitiveValue
    {
        ValueType getRealValue();
    }
}