using System;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public interface IJsonValidator
    {
        bool isSyntacticalValid(String json);
        bool isSemanticalValid(String json);
    }
}