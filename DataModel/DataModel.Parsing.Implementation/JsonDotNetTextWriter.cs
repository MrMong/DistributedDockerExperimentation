using System;
using System.IO;
using System.Text;
using DistributedExperimentation.DataModel.Implementation;
using Newtonsoft.Json;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public class JsonDotNetTextWriter : IJsonWriter
    {
        private JsonTextWriter jwriter;
        private StringBuilder sbuilder;

        private JsonDotNetTextWriter(StringBuilder sbuilder) {
            bool isOk = (sbuilder != null);
            if (isOk) {
                this.sbuilder = sbuilder;
                this.jwriter = new JsonTextWriter(new StringWriter(sbuilder));
                this.jwriter.Formatting = Formatting.Indented;
            } else {
                throw new ArgumentException("Argument 'sbuilder' must be a " +
                                            "not null StringBuilder object.");                
            }
        }

        public static JsonDotNetTextWriter create(StringBuilder sbuilder) {
            return new JsonDotNetTextWriter(sbuilder);
        }

        public StringBuilder getStringBuilder() {
            return new StringBuilder(this.sbuilder.ToString());
        }

        public void clear() 
        {
            this.sbuilder.Clear();
        }

        public void writeComment(string comment)
        {
            this.jwriter.WriteComment(comment);
        }

        public void writeEndArray()
        {
            this.jwriter.WriteEndArray();
        }

        public void writeEndObject()
        {
            this.jwriter.WriteEndObject();
        }

        public void writeValue(object value)
        {
            this.jwriter.WriteValue(value);
        }       

        public void writePropertyName(string name)
        {
            this.jwriter.WritePropertyName(name);
        }

        public void writeStartArray()
        {
            this.jwriter.WriteStartArray();
        }

        public void writeStartObject()
        {
            this.jwriter.WriteStartObject();
        }

        override
        public String ToString() 
        {
            return this.sbuilder.ToString();
        }
    }
}