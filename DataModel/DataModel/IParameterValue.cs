using System;

namespace DistributedExperimentation.DataModel
{
    public interface IParameterValue
    {
        bool isPrimitive();
        String getValueTypeName();
    }
}