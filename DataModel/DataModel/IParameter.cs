using System;

namespace DistributedExperimentation.DataModel
{
    public interface IParameter
    {
        String getId();
        String getName();
        String getDescription();
        IParameterValue getValue();
    }
}