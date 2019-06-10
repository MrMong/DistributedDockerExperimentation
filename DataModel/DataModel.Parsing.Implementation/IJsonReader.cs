using System;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public interface IJsonReader<T>
    {
        void initialize(String jsonString);
        bool readNextToken();
        T getCurrentTokenType();
        object getCurrentValue();
        void close();
    }
}