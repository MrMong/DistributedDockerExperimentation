using System;

namespace DistributedExperimentation.DataModel
{
    public interface IParameterCollection : IParameterValue
    {
        String getId();
        String getName();
        String getDescription();
        void clear();
        uint count();
        bool isEmpty();
    }
}