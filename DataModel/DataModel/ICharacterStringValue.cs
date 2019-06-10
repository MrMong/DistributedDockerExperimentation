using System;

namespace DistributedExperimentation.DataModel
{
    public interface ICharacterStringValue : IPrimitiveValue
    {
        String getCharacterStringValue();
    }
}