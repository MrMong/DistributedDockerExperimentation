using System;

namespace DistributedExperimentation.DataModel
{
    public interface IParameterList : IParameterCollection
    {   
        void add(IParameter parameter);
        void remove(IParameter parameter);
        uint indexOf(IParameter parameter);
        void insert(uint index, IParameter parameter);
        bool contains(IParameter parameter);
        void removeAt(uint index);
        IParameter get(uint index);
        IParameter first();
        IParameter last();
    }
}