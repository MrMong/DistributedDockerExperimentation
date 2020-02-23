using System;
using System.IO;
using System.Text;
using DistributedExperimentation.DataModel.Implementation;
using Manatee.Json;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public class JsonManateeValidator : IJsonValidator
    {
        public enum SchemaVersion
        {
            All,
            Draft04,
            Draft06,
            Draft07,
            Draft08,
            None
        }

        private JsonSchema schema;
        private MetaSchemaValidationResults meta;

        private JsonManateeValidator(String schema) 
        {
            bool isOk = (!String.IsNullOrEmpty(schema) &&
                        !String.IsNullOrWhiteSpace(schema));
            if (isOk) {
                if (this.isSyntacticalValid(schema)) {
                    JsonValue jsonObj = JsonValue.Parse(schema);
                    bool isValid = false;
                    if (this.isSemanticalValidSchema(schema)){
                        JsonSchema schemaObj = new JsonSerializer().Deserialize<JsonSchema>(jsonObj);
                        MetaSchemaValidationResults metaResult = schemaObj.ValidateSchema();
                        if (metaResult.IsValid) {
                            this.schema = schemaObj;
                            this.meta = metaResult;
                            isValid = true;
                        }
                    }
                    if (!isValid) {
                        throw new ArgumentException("Argument 'schema' must have a " +
                                                    "valid Json Schema format.");                    
                    }
                } else {
                    throw new ArgumentException("Argument 'schema' must " +
                                                "be a valid Json string.");
                }
            } else {
                throw new ArgumentException("Argument 'schema' must be a not null " +
                                            "and a not empty String object, that " +
                                            "have not been only whitspace.");             
            }
        }

        public static JsonManateeValidator create(String schema) 
        {
            return new JsonManateeValidator(schema);
        }

        public bool isSyntacticalValid(string json)
        {
            bool result = false;
            bool isOk = (!String.IsNullOrEmpty(json) &&
                        !String.IsNullOrWhiteSpace(json));
            if (isOk) {
                try {
                    JsonValue.Parse(json);
                    result = true;
                } catch (JsonSyntaxException) {
                    result = false;
                }
            } else {
                throw new ArgumentException("Argument 'json' must be a not null " +
                                            "and a not empty String object, that " +
                                            "have not been only whitspace.");              
            }
            return result;
        }          

        public bool isSemanticalValid(string json)
        {
            bool result = false;
            if(isSyntacticalValid(json)) {
                result = this.schema.Validate(JsonValue.Parse(json)).IsValid;
            }
            return result;
        }    

        public SchemaVersion getSupportedVersions()
        {
            return this.mapSchemaVersion(this.meta.SupportedVersions);
        }

        private bool isSemanticalValidSchema(String schema) 
        {
            bool result = false;
            if(isSyntacticalValid(schema)) {
                try {
                    new JsonSerializer().Deserialize<JsonSchema>(JsonValue.Parse(schema));
                    result = true;
                } catch (Exception) {
                    result = false;
                }
            }
            return result;
        }        

        private SchemaVersion mapSchemaVersion(JsonSchemaVersion version) 
        {
            SchemaVersion mappedVersion = SchemaVersion.None;
            if (version == JsonSchemaVersion.All)
                mappedVersion = SchemaVersion.All;
            if (version == JsonSchemaVersion.Draft04)
                mappedVersion = SchemaVersion.Draft04;
            if (version == JsonSchemaVersion.Draft06)
                mappedVersion = SchemaVersion.Draft06;
            if (version == JsonSchemaVersion.Draft07)
                mappedVersion = SchemaVersion.Draft07;
            if (version == JsonSchemaVersion.Draft2019_09)
                mappedVersion = SchemaVersion.Draft08;
            return mappedVersion;
        }
    }
}