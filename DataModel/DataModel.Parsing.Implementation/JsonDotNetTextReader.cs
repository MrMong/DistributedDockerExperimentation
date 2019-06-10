using System;
using System.IO;
using System.Text;
using DistributedExperimentation.DataModel.Implementation;
using Newtonsoft.Json;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public class JsonDotNetTextReader : IJsonReader<TokenType>
    {
        private StringReader sreader;
        private JsonTextReader jreader;

        private JsonDotNetTextReader(){}

        public static JsonDotNetTextReader create() {
            return new JsonDotNetTextReader();
        }

        public void initialize(string jsonString)
        {
            bool isOk = (jsonString != null);
            if (isOk) {
                this.close();
                this.sreader = new StringReader(jsonString);
                this.jreader = new JsonTextReader(this.sreader);
            } else {
                throw new ArgumentException("Argument 'jsonString' must " +
                                            "be a not null String object.");
            }
        }

        public bool readNextToken()
        {
            bool result = false;
            if (this.jreader != null)
                result = this.jreader.Read();
            return result;
        }

        public TokenType getCurrentTokenType()
        {
            TokenType result = TokenType.None;
            if (this.jreader != null)
                result = this.mapTokenType(this.jreader.TokenType);
            return result;
        }

        public object getCurrentValue()
        {
            object result = null;
            if (this.jreader != null)
                result = this.jreader.Value;
            return result;
        }

        public void close()
        {
            if (this.sreader != null)
                this.sreader.Close();
            if (this.jreader != null)
                this.jreader.Close();
        }

        private TokenType mapTokenType(JsonToken type) 
        {
            TokenType mappedType = TokenType.Undefined;
            if (type == JsonToken.None)
                mappedType = TokenType.None;
            if (type == JsonToken.StartObject)
                mappedType = TokenType.StartObject;
            if (type == JsonToken.StartArray)
                mappedType = TokenType.StartArray;
            if (type == JsonToken.PropertyName)
                mappedType = TokenType.PropertyName;
            if (type == JsonToken.Comment)
                mappedType = TokenType.Comment;
            if (type == JsonToken.Raw)
                mappedType = TokenType.Raw;
            if (type == JsonToken.Integer)
                mappedType = TokenType.Integer;
            if (type == JsonToken.Float)
                mappedType = TokenType.Float;
            if (type == JsonToken.String)
                mappedType = TokenType.String;
            if (type == JsonToken.Boolean)
                mappedType = TokenType.Boolean;
            if (type == JsonToken.Null)
                mappedType = TokenType.Null;
            if (type == JsonToken.EndObject)
                mappedType = TokenType.EndObject;
            if (type == JsonToken.EndArray)
                mappedType = TokenType.EndArray;
            return mappedType;
        }
    }
}