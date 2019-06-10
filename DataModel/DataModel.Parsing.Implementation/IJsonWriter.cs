using System;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public interface IJsonWriter
    {
        void writeStartObject();
        void writeEndObject();
        void writePropertyName(String name);
        void writeValue(object value);
        void writeComment(String comment);
        void writeStartArray();
        void writeEndArray();
        void clear();
        String ToString();
    }
}