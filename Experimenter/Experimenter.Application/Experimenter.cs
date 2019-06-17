using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DistributedExperimentation.DataModel;
using DistributedExperimentation.DataModel.Parsing;
using DistributedExperimentation.DataModel.Parsing.Implementation;
using DistributedExperimentation.Experimenter.ExecutorPlugin;

namespace DistributedExperimentation.Experimenter.Application
{
    public class Experimenter
    {
        private ExperimentSeriesObjectBuilder obuilder;
        private ExperimentSeriesJsonBuilder jbuilder;
        private ExperimentSeriesObjectParser oparser;
        private ExperimentSeriesJsonParser jparser;

        private Experimenter() 
        {
            this.obuilder = ExperimentSeriesObjectBuilder.create();
            this.oparser = ExperimentSeriesObjectParser.create(obuilder);
            this.jbuilder = createExperimentSeriesJsonBuilder();
            this.jparser = createExperimentSeriesJsonParser(obuilder);                  
        }

        public static Experimenter create() 
        {
            return new Experimenter();
        }

        public void executeExperimentation(String jsonExperimentSeries, IExecutorPlugin executorPlugin) 
        {
            if (jsonExperimentSeries == null) {
                throw new ArgumentException("Argument 'jsonExperimentSeries' must " +
                                            "be a not null string object.");              
            }  
            if (executorPlugin == null) {
                throw new ArgumentException("Argument 'executorPlugin' must be a not null.");              
            }                      
            if (!this.jparser.getJsonValidator().isSyntacticalValid(jsonExperimentSeries)) {
                throw new ArgumentException("Argument 'jsonExperimentSeries' must " +
                                            "be a syntactically valid JSON string.");                
            }
            if (!this.jparser.getJsonValidator().isSemanticalValid(jsonExperimentSeries)) {
                throw new ArgumentException("Argument 'jsonExperimentSeries' must " +
                                            "be a semantically valid JSON string.");
            }
            IExperimentSeries eSeries = (IExperimentSeries)this.jparser.parse(jsonExperimentSeries);
            this.jbuilder.reset();
            this.obuilder.reset();
            executorPlugin.execute(eSeries);
        }

        public static String getCurrentJsonSchema()
        {
            return ExperimentSeriesJsonParser.getCurrentJsonSchema();
        }

        private ExperimentSeriesJsonBuilder createExperimentSeriesJsonBuilder() 
        {
            JsonDotNetTextWriter jwriter = JsonDotNetTextWriter.create(new StringBuilder());
            return ExperimentSeriesJsonBuilder.create(jwriter);
        }

        private ExperimentSeriesJsonParser createExperimentSeriesJsonParser(IExperimentSeriesBuilder builder) 
        {
            JsonDotNetTextReader jreader = JsonDotNetTextReader.create();
            String schema = ExperimentSeriesJsonParser.getCurrentJsonSchema();
            JsonManateeValidator jvalidator = JsonManateeValidator.create(schema);
            return ExperimentSeriesJsonParser.create(builder, jreader, jvalidator);
        }        
    }
}