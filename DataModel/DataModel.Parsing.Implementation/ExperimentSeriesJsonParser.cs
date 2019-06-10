using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistributedExperimentation.DataModel.Implementation;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public class ExperimentSeriesJsonParser : IExperimentSeriesParser
    {
        private IExperimentSeriesBuilder builder;
        private IJsonReader<TokenType> jreader;
        private IJsonValidator jvalidator;

        private ExperimentSeriesJsonParser(IExperimentSeriesBuilder builder, 
                                            IJsonReader<TokenType> jreader,
                                            IJsonValidator jvalidator) {
            bool isOk = ((builder != null) &&
                        (jreader != null) &&
                        (jvalidator != null));
            if (isOk) {
                this.builder = builder;
                this.jreader = jreader;
                this.jvalidator = jvalidator;
            } else {
                throw new ArgumentException("Arguments 'builder', 'jreader' " + 
                                            "and 'jvalidator' must be not null.");
            }
        }

        public static ExperimentSeriesJsonParser create(IExperimentSeriesBuilder builder, 
                                                        IJsonReader<TokenType> jreader,
                                                        IJsonValidator jvalidator) {
            return new ExperimentSeriesJsonParser(builder, jreader, jvalidator);
        }

        public IExperimentSeriesBuilder getBuilder()
        {
            return this.builder;
        }

        public IJsonReader<TokenType> getJsonReader()
        {
            return this.jreader;
        }

        public IJsonValidator getJsonValidator()
        {
            return this.jvalidator;
        }

        public object parse(object parseObject)
        {
            bool isOk = ((parseObject != null) &&
                        (parseObject is String));     
            object result = null;
            if (isOk) {
                String parseString = (String)parseObject;
                if (this.jvalidator.isSemanticalValid(parseString)) {
                    result = this.parse(parseString);
                } else {
                    throw new ArgumentException("Argument 'parseObject' must be " +
                                                "a syntactically and semantically " +
                                                "valid JSON string.");                      
                }
            } else {
                throw new ArgumentException("Argument 'parseObject' must " +
                                            "be a not null String object.");      
            }
            return result;
        }

        private object parse(String parseObject) 
        {
            this.jreader.initialize(parseObject);
            this.jreader.readNextToken();
            TokenType type = this.jreader.getCurrentTokenType();      
            bool isOk = (type == TokenType.StartObject);
            if (isOk) {
                IExperimentSeries expSer = this.parseExperimentSeries(this.jreader);
                jreader.readNextToken();
                this.builder.setId(expSer.getId());
                this.builder.setName(expSer.getName());
                this.builder.setDescription(expSer.getDescription());
                this.builder.addRange(expSer.getExperiments());
                this.builder.setSoftwareName(expSer.getExperimentSoftware());  
            } else {
                throw new ArgumentException("Argument 'type' must be a " +
                                            "TokenType with option of StartObject.");             
            }
            object result = this.builder.build();
            this.builder.reset();
            return result;
        }

        private IExperimentSeries parseExperimentSeries(IJsonReader<TokenType> jreader) 
        {
            if (jreader.getCurrentTokenType() == TokenType.StartObject)
                jreader.readNextToken();
            String sid = "";
            String name = "";
            String desc = "";
            String esof = "";
            List<IExperiment> expList = new List<IExperiment>();
            do {
                TokenType type = jreader.getCurrentTokenType();
                object value = jreader.getCurrentValue();
                bool isOk = ((type == TokenType.PropertyName) &&
                            (value != null) &&
                            (value is String)); 
                if (isOk) {
                    if (String.Compare((String)value, "series_id") == 0) {
                        jreader.readNextToken();
                        sid = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "name") == 0) {
                        jreader.readNextToken();
                        name = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "description") == 0) {
                        jreader.readNextToken();
                        desc = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "experiment_software") == 0) {
                        jreader.readNextToken();
                        esof = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "experiments") == 0) {
                        jreader.readNextToken();
                        if (jreader.getCurrentTokenType() != TokenType.StartArray)
                            throw new ArgumentException("TokenType must be a StartArray."); 
                        do {
                            jreader.readNextToken();
                            expList.Add(this.parseExperiment(jreader));
                            jreader.readNextToken();
                        } while(TokenType.EndArray != jreader.getCurrentTokenType());                        
                    } else {
                        this.skipJsonField(jreader);
                    }
                    jreader.readNextToken();                                                   
                } else {
                    throw new ArgumentException("Argument 'type' must be a TokenType " +
                                                "with option of PropertyName.\n" +
                                                "Argument 'value' must be not null " +
                                                "and of type String.");             
                }                                    
            } while(TokenType.EndObject != jreader.getCurrentTokenType());
            return ExperimentSeries.create(sid, name, desc, expList, esof);                
        }

        
        private IExperiment parseExperiment(IJsonReader<TokenType> jreader) {
            if (jreader.getCurrentTokenType() == TokenType.StartObject)
                jreader.readNextToken();
            String eid = "";
            String name = "";
            String desc = "";
            IParameterList plist = null;
            do {
                TokenType type = jreader.getCurrentTokenType();
                object value = jreader.getCurrentValue();
                bool isOk = ((type == TokenType.PropertyName) &&
                            (value != null) &&
                            (value is String));
                if (isOk) {
                    if (String.Compare((String)value, "experiment_id") == 0) {
                        jreader.readNextToken();
                        eid = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "name") == 0) {
                        jreader.readNextToken();
                        name = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "description") == 0) {
                        jreader.readNextToken();
                        desc = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "parametercollection") == 0) {
                        jreader.readNextToken();
                        plist = this.parseParameterList(jreader);
                    } else {
                        this.skipJsonField(jreader);
                    }
                    jreader.readNextToken();
                } else {
                    throw new ArgumentException("Argument 'type' must be a TokenType " +
                                                "with option of PropertyName.\n" +
                                                "Argument 'value' must be not null " +
                                                "and of type String.");                     
                }
            } while(TokenType.EndObject != jreader.getCurrentTokenType());
            return Experiment.create(eid, name, desc, plist);
        }

        private IParameter parseParameter(IJsonReader<TokenType> jreader) {
            if (jreader.getCurrentTokenType() == TokenType.StartObject)
                jreader.readNextToken();
            String pid = "";
            String name = "";
            String desc = "";
            IParameterValue paramValue = null;
            do {
                TokenType type = jreader.getCurrentTokenType();
                object value = jreader.getCurrentValue();
                bool isOk = ((type == TokenType.PropertyName) &&
                            (value != null) &&
                            (value is String));
                if (isOk) {
                    if (String.Compare((String)value, "parameter_id") == 0) {
                        jreader.readNextToken();
                        pid = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "name") == 0) {
                        jreader.readNextToken();
                        name = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "description") == 0) {
                        jreader.readNextToken();
                        desc = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "value") == 0) {
                        jreader.readNextToken();
                        paramValue = this.parseParameterValue(jreader);
                    } else {
                        this.skipJsonField(jreader);
                    }
                    jreader.readNextToken();
                } else {
                    throw new ArgumentException("Argument 'type' must be a TokenType " +
                                                "with option of PropertyName.\n" +
                                                "Argument 'value' must be not null " +
                                                "and of type String.");
                }
            } while(TokenType.EndObject != jreader.getCurrentTokenType());
            return Parameter.create(pid, name, desc, paramValue);
        }

        private IParameterValue parseParameterValue(IJsonReader<TokenType> jreader)
        {
            TokenType type = jreader.getCurrentTokenType();
            object value = jreader.getCurrentValue();            
            bool isPrimitive = ((type == TokenType.String) ||
                        (type == TokenType.Boolean) ||
                        (type == TokenType.Integer) ||
                        (type == TokenType.Float));
            bool isComplex = ((type == TokenType.StartObject));
            IParameterValue pvalue = null;
            if (isPrimitive || isComplex) {
                if (isPrimitive) {
                    pvalue = this.parsePrimitiveValue(jreader);
                } else if (isComplex) {
                    pvalue = this.parseParameterList(jreader);
                }
            } else {
                throw new ArgumentException("Current JSON token must have a TokenType " +
                                            "with option of String or Boolean " +
                                            "or Integer or Float or StartObject\n" +
                                            "and a not null Value of type " +
                                            "String or Boolean or ValueType.");                
            }
            return pvalue;
        }

        private IParameterList parseParameterList(IJsonReader<TokenType> jreader)
        {
            if (jreader.getCurrentTokenType() == TokenType.StartObject)
                jreader.readNextToken();
            String cid = "";
            String name = "";
            String desc = "";
            List<IParameter> plist = new List<IParameter>();
            do {
                TokenType type = jreader.getCurrentTokenType();
                object value = jreader.getCurrentValue();                       
                bool isOk = ((type == TokenType.PropertyName) &&
                            (value != null) &&
                            (value is String));
                if (isOk) {
                    if (String.Compare((String)value, "collection_id") == 0) {
                        jreader.readNextToken();
                        cid = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "name") == 0) {
                        jreader.readNextToken();
                        name = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "description") == 0) {
                        jreader.readNextToken();
                        desc = (String)jreader.getCurrentValue();
                    } else if (String.Compare((String)value, "parameters") == 0) {
                        jreader.readNextToken();
                        if (jreader.getCurrentTokenType() != TokenType.StartArray)
                            throw new ArgumentException("TokenType must be a StartArray."); 
                        do {
                            jreader.readNextToken();
                            plist.Add(this.parseParameter(jreader));
                            jreader.readNextToken();
                        } while(TokenType.EndArray != jreader.getCurrentTokenType());                        
                    } else {
                        this.skipJsonField(jreader);
                    }
                    jreader.readNextToken();       
                } else {
                    throw new ArgumentException("Current JSON token must have a " +
                                                "TokenType with option of StartArray.");                    
                }
            } while (TokenType.EndObject != jreader.getCurrentTokenType());
            return ParameterArrayList.create(cid, name, desc, plist);  
        }

        private IPrimitiveValue parsePrimitiveValue(IJsonReader<TokenType> jreader)
        { 
            TokenType type = jreader.getCurrentTokenType();
            object value = jreader.getCurrentValue();                       
            bool isOk = (((type == TokenType.String) ||
                        (type == TokenType.Boolean) ||
                        (type == TokenType.Integer) ||
                        (type == TokenType.Float)) &&
                        (value != null) &&
                        ((value is String) ||  
                        (value is Boolean) || 
                        (value is ValueType)));
            IPrimitiveValue pvalue = null;
            if (isOk) {
                if (type == TokenType.String)
                    pvalue = CharacterStringValue.create((String)value);
                if (type == TokenType.Integer)
                    pvalue = IntegerValue.create((ValueType)value);                    
                if (type == TokenType.Float)
                    pvalue = RealValue.create((ValueType)value);
                if (type == TokenType.Boolean)
                    pvalue = BooleanValue.create((Boolean)value);           
            } else {
                throw new ArgumentException("Argument 'type' must be a TokenType with " +
                                            "option of String or Boolean " +
                                            "or Integer or Float\n" +
                                            "Argument 'value' must be not null and " +
                                            "of type String or Boolean or ValueType.");  
            }
            return pvalue;
        }

        private void skipJsonField(IJsonReader<TokenType> jreader)
        {
            bool isBreaked = false;
            do {
                jreader.readNextToken();
                TokenType type = jreader.getCurrentTokenType();
                isBreaked = ((type == TokenType.EndArray) ||
                            (type == TokenType.EndObject) ||
                            (type == TokenType.Float) || 
                            (type == TokenType.String) ||
                            (type == TokenType.Integer) ||
                            (type == TokenType.Boolean) ||
                            (type == TokenType.None) ||
                            (type == TokenType.Raw) ||
                            (type == TokenType.Null) ||
                            (type == TokenType.Undefined));
                
            } while (!isBreaked);
        }

        public static String getCurrentJsonSchema()
        {
            return @"
            {
                ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                ""title"": ""ExperimentSeries"",
                ""description"": ""A amount of experiments with parameters for execution of experiment software."",

                ""definitions"": {
                    ""id_value"": {
                        ""type"": ""string"",
                        ""minLength"": 1,
                        ""maxLength"": 60,
                        ""pattern"": ""^([A-Za-z0-9])+$""
                    },
                    ""characterstring_value"": {
                        ""type"": ""string"", ""minLength"": 1
                    },
                    ""integer_value"": {
                        ""type"": ""number"", ""multipleOf"": 1
                    },
                    ""real_value"": {
                        ""type"": ""number""
                    },
                    ""boolean_value"": {
                        ""type"": ""boolean""
                    },                                                                                                             
                    ""parameter"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""parameter_id"": { ""$ref"": ""#/definitions/id_value"" },
                            ""name"": { ""type"": ""string"", ""minLength"": 0 },
                            ""description"": { ""type"": ""string"" },
                            ""is_primitive"": { ""type"": ""boolean"" },
                            ""value_type"": { 
                                ""type"": ""string"",
                                ""enum"": [
                                    ""characterstring"",
                                    ""integer"",
                                    ""real"",
                                    ""boolean"",
                                    ""parametercollection""
                                ]
                            },       
                            ""value"": {
                            ""anyOf"": [
                                { ""$ref"": ""#/definitions/characterstring_value"" },
                                { ""$ref"": ""#/definitions/integer_value"" },
                                { ""$ref"": ""#/definitions/real_value"" },
                                { ""$ref"": ""#/definitions/boolean_value"" },
                                { ""$ref"": ""#/definitions/parameter_list"" }
                            ]
                            }
                        },
                        ""allOf"": [
                        { 
                            ""if"": {
                                ""properties"": {
                                    ""value_type"": { ""const"": ""characterstring"" }
                                }
                            },
                            ""then"": {
                                ""properties"": {
                                    ""value"": { ""$ref"": ""#/definitions/characterstring_value"" }
                                }
                            }
                        },
                        {
                            ""if"": {
                                ""properties"": {
                                    ""value_type"": { ""const"": ""integer"" }
                                }
                            },
                            ""then"": {
                                ""properties"": {
                                    ""value"": { ""$ref"": ""#/definitions/integer_value"" }
                                }
                            }
                        },
                        {
                            ""if"": {
                                ""properties"": {
                                    ""value_type"": { ""const"": ""real"" }
                                }
                            },
                            ""then"": {
                                ""properties"": {
                                    ""value"": { ""$ref"": ""#/definitions/real_value"" }
                                }
                            }
                        },
                        { 
                            ""if"": {
                                ""properties"": {
                                    ""value_type"": { ""const"": ""boolean"" }
                                }
                            },
                            ""then"": {
                                ""properties"": {
                                    ""value"": { ""$ref"": ""#/definitions/boolean_value"" }
                                }
                            }
                        },
                        { 
                            ""if"": {
                                ""properties"": {
                                    ""value_type"": { ""const"": ""parametercollection"" }
                                }
                            },
                            ""then"": {
                                ""properties"": {
                                    ""value"": { ""$ref"": ""#/definitions/parameter_list"" }
                                }
                            }                            
                        }
                        ],                        
                        ""required"": [""name"",""value"",""parameter_id"",""value_type"",""is_primitive""],
                        ""additionalProperties"" : false
                    },
                    ""parameter_list"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""collection_id"": { ""$ref"": ""#/definitions/id_value"" },
                            ""name"": { ""type"": ""string"" },
                            ""description"": { ""type"": ""string"" },                     
                            ""parameters"": {
                                ""description"": ""The list of parameters they will be used for experiment."",
                                ""type"": ""array"",
                                ""items"": { ""$ref"": ""#/definitions/parameter"" },
                                ""minItems"": 1,
                                ""additionalItems"": false 
                            }
                        },
                        ""required"": [""collection_id"",""parameters""],
                        ""additionalProperties"" : false                 
                    },
                    ""experiment"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""experiment_id"": { ""$ref"": ""#/definitions/id_value"" },
                            ""name"": { ""type"": ""string"" },
                            ""description"": { ""type"": ""string"" },                    
                            ""parametercollection"": { ""$ref"": ""#/definitions/parameter_list"" }
                        },
                        ""required"": [""experiment_id"",""parametercollection""],
                        ""additionalProperties"" : false
                    },
                    ""experiment_series"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""series_id"": { ""$ref"": ""#/definitions/id_value"" },
                            ""name"": { ""type"": ""string"" },
                            ""description"": { ""type"": ""string"" },
                            ""experiment_software"": { ""type"": ""string"", ""minLength"": 1 },
                            ""experiments"": {
                                ""description"": ""The list of experiments they should be done."",
                                ""type"": ""array"",
                                ""items"": { ""$ref"": ""#/definitions/experiment"" },
                                ""minItems"": 1,
                                ""additionalItems"": false 
                            }
                        },
                        ""required"": [""series_id"",""experiments"",""experiment_software""],
                        ""additionalProperties"" : false
                    }
                },

                ""$ref"": ""#/definitions/experiment_series""
            }              
            ";
        }
    }
}