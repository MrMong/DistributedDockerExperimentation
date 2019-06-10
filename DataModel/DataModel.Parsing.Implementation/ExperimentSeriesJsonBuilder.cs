using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DistributedExperimentation.DataModel.Implementation;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public class ExperimentSeriesJsonBuilder : ExperimentSeriesBuilder
    {
        private IJsonWriter jwriter;

        private ExperimentSeriesJsonBuilder(IJsonWriter jwriter) 
        {
            bool isOk = (jwriter != null);
            if (isOk) {
                this.jwriter = jwriter;
            } else  {
                throw new ArgumentException("Argument 'jwriter' must be a not null " +
                                            "implementation of IJsonWriter object.");            
            }
            this.experiments = new List<IExperiment>();
        }

        public static ExperimentSeriesJsonBuilder create(IJsonWriter jwriter) 
        {
            return new ExperimentSeriesJsonBuilder(jwriter);
        }       

        override
        public object build()
        {
            bool isOk = ((this.id != null) && (this.name != null) &&
            (this.description != null) && 
            (this.softwarename != null));
            String json = "";
            if (isOk) {
                this.addExperimentSeries(this.id, this.name, this.description, 
                                         this.experiments, this.softwarename);
                json = this.jwriter.ToString();
            } else {
                throw new ArgumentException("Properties 'id', 'name', 'description', " +
                                            "'experiments' and 'softwarename' of " +
                                            "ExperimentSeries must be setted completely.");
            }
            return json;
        }

        override
        public void reset() 
        {
            this.id = null;
            this.name = null;
            this.description = null;
            this.softwarename = null;
            experiments.Clear();
            this.jwriter.clear();      
        }

        public IJsonWriter getJsonWriter() 
        {
            return this.jwriter;
        }

        private void addExperimentSeries(String id, String name, String description, 
                                         IList<IExperiment> experiments, String softwarename) 
        {
            this.jwriter.writeStartObject();
            this.jwriter.writePropertyName("series_id");
            this.jwriter.writeValue(id);
            if (!String.IsNullOrEmpty(name)) {
                this.jwriter.writePropertyName("name");
                this.jwriter.writeValue(name);
            }
            if (!String.IsNullOrEmpty(description)) {
                this.jwriter.writePropertyName("description");
                this.jwriter.writeValue(description);
            }
            this.jwriter.writePropertyName("experiment_software");
            this.jwriter.writeValue(softwarename);     
            this.jwriter.writePropertyName("experiments");
            this.jwriter.writeStartArray();
            foreach(IExperiment exp in experiments) {
                this.addExperiment(exp);
            }
            this.jwriter.writeEndArray();
            this.jwriter.writeEndObject();   
        }

        private void addExperiment(IExperiment experiment) 
        {
            bool isOk = (experiment.getParameters() is IParameterList);
            if (isOk) {

                this.jwriter.writeStartObject();
                this.jwriter.writePropertyName("experiment_id");
                this.jwriter.writeValue(experiment.getId());
                if (!String.IsNullOrEmpty(experiment.getName())) {
                    this.jwriter.writePropertyName("name");
                    this.jwriter.writeValue(experiment.getName());
                }
                if (!String.IsNullOrEmpty(experiment.getDescription())) {
                    this.jwriter.writePropertyName("description");
                    this.jwriter.writeValue(experiment.getDescription());
                }                
                this.jwriter.writePropertyName("parametercollection");
                this.addParameterCollection((IParameterList)experiment.getParameters());
                this.jwriter.writeEndObject();
            } else {
                throw new ArgumentException("Property 'experiment.getParameters()' of argument " +
                                            "'experiment' must be of type IParameterList.");
            }
        }

        private void addParameter(IParameter parameter) 
        {
            if (parameter != null) {
                String name = parameter.getName();
                if (name == null)
                    name = "";
                this.jwriter.writeStartObject();
                this.jwriter.writePropertyName("parameter_id");
                this.jwriter.writeValue(parameter.getId());
                this.jwriter.writePropertyName("name");
                this.jwriter.writeValue(name);
                if (!String.IsNullOrEmpty(parameter.getDescription())) {
                    this.jwriter.writePropertyName("description");
                    this.jwriter.writeValue(parameter.getDescription());
                }
                this.jwriter.writePropertyName("is_primitive");
                this.jwriter.writeValue(parameter.getValue().isPrimitive());         
                this.jwriter.writePropertyName("value_type");
                this.jwriter.writeValue(parameter.getValue().getValueTypeName());                       
                this.jwriter.writePropertyName("value");
                this.addParameterValue(parameter.getValue());
                this.jwriter.writeEndObject();
            } else  {
                throw new ArgumentException("Argument 'parameter' must be not null.");             
            }
        }

        private void addParameterValue(IParameterValue value)
        {
            if (value != null) {
                if (value.isPrimitive()) {
                    this.addPrimitiveParameterValue((IPrimitiveValue)value);
                } else {
                    this.addParameterCollection((IParameterCollection)value);
                }
            } else  {
                throw new ArgumentException("Argument 'value' must be not null and " +
                                            "an implemented IParameterValue object.");            
            }
        }

        private void addPrimitiveParameterValue(IPrimitiveValue value) 
        {
            bool isOk = ((value != null) && (
                        (value is IntegerValue) || 
                        (value is RealValue) || 
                        (value is BooleanValue) || 
                        (value is CharacterStringValue)));
            if (isOk) {
                if (value is IntegerValue)
                    this.jwriter.writeValue(((IntegerValue)value).getIntegerValue());
                if (value is RealValue)
                    this.jwriter.writeValue(((RealValue)value).getRealValue());
                if (value is BooleanValue)
                    this.jwriter.writeValue(((BooleanValue)value).getBooleanValue());
                if (value is CharacterStringValue)
                    this.jwriter.writeValue(((CharacterStringValue)value).getCharacterStringValue());              
            } else  {
                throw new ArgumentException("Argument 'value' must be not " +
                                            "null and an object of type\n" +
                                            "IntegerValue or BooleanValue or " +
                                            "RealValue or CharacterStringValue.");            
            }
        }

        private void addParameterCollection(IParameterCollection list) 
        {
            bool isOk = ((list != null) && 
                        (list is IParameterList));
            if (isOk) {
                this.jwriter.writeStartObject();
                this.jwriter.writePropertyName("collection_id");
                this.jwriter.writeValue(list.getId());
                if (!String.IsNullOrEmpty(list.getName())) {
                    this.jwriter.writePropertyName("name");
                    this.jwriter.writeValue(list.getName());
                }
                if (!String.IsNullOrEmpty(list.getDescription())) {
                    this.jwriter.writePropertyName("description");
                    this.jwriter.writeValue(list.getDescription());
                }
                this.jwriter.writePropertyName("parameters");
                this.jwriter.writeStartArray();
                for(uint i=0; i<list.count(); i++) {
                    this.addParameter(((IParameterList)list).get(i));
                }
                this.jwriter.writeEndArray();
                this.jwriter.writeEndObject();
            } else  {
                throw new ArgumentException("Argument 'list' must be not null and " +
                                            "an implemented IParameterList object.");            
            }
        }

    }
}