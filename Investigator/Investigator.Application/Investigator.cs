using System;
using System.Text;
using System.Threading.Tasks;
using DistributedExperimentation.DataModel;
using DistributedExperimentation.DataModel.Parsing;
using DistributedExperimentation.DataModel.Parsing.Implementation;

namespace DistributedExperimentation.Investigator.Application
{
    public class Investigator
    {
        private ExperimentSeriesObjectBuilder obuilder;
        private ExperimentSeriesJsonBuilder jbuilder;
        private ExperimentSeriesObjectParser oparser;
        private ExperimentSeriesJsonParser jparser;
        private ExperimentDistributor distributor;

        private Investigator(Uri dockerHost, Version dockerRemoteApiVersion) 
        {
            bool isOk = ((dockerHost != null) && 
                         (dockerHost.IsWellFormedOriginalString()));
            if (isOk) {
                this.obuilder = ExperimentSeriesObjectBuilder.create();
                this.oparser = ExperimentSeriesObjectParser.create(obuilder);
                this.jbuilder = createExperimentSeriesJsonBuilder();
                this.jparser = createExperimentSeriesJsonParser(obuilder);
                this.distributor = createExperimentDistributor(this.jbuilder,
                                                               dockerHost,
                                                               dockerRemoteApiVersion);
            } else {
                throw new ArgumentException("Argument 'dockerHost' must be not null " +
                                            "and a well-formed uri string.");
            }
        }

        public static Investigator create(Uri dockerHost, Version dockerRemoteApiVersion) 
        {
            return new Investigator(dockerHost, dockerRemoteApiVersion);
        }

        public Task startExperimentation(String jsonExperimentSeries, 
                                         String dockerImage, 
                                         String executionPath) 
        {
            if (jsonExperimentSeries == null) {
                throw new ArgumentException("Argument 'jsonExperimentSeries' must " +
                                            "be a not null string object.");              
            }
            if (dockerImage == null) {
                throw new ArgumentException("Argument 'dockerImage' must " +
                                            "be a not null string object.");              
            }
            if (executionPath == null) {
                throw new ArgumentException("Argument 'executionPath' must " +
                                            "be a not null string object.");     
            }            
            if (!this.isSyntacticalValidJson(jsonExperimentSeries)) {
                throw new FormatException("Argument 'jsonExperimentSeries' must " +
                                          "be a syntactically valid JSON string.");                
            }
            if (!this.isSemanticalValidJson(jsonExperimentSeries)) {
                throw new ApplicationException("Argument 'jsonExperimentSeries' must " +
                                            "be a semantically valid JSON string.");
            }
            IExperimentSeries eSeries = (IExperimentSeries)this.jparser.parse(jsonExperimentSeries);
            this.jbuilder.reset();
            this.obuilder.reset();
            return this.distributor.distributeExperimentSeries(eSeries, dockerImage, executionPath);
        }

        public bool isSyntacticalValidJson(String json)
        {
            return this.jparser.getJsonValidator().isSyntacticalValid(json);
        }
        public bool isSemanticalValidJson(String json) 
        {
            return this.jparser.getJsonValidator().isSemanticalValid(json);
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

        private ExperimentDistributor createExperimentDistributor(IExperimentSeriesBuilder builder, 
                                                                  Uri dockerHost,
                                                                  Version dockerRemoteApiVersion)
        {
            ExperimentDockerClient client = null;
            if (dockerRemoteApiVersion != null)
                client = ExperimentDockerClient.create(dockerHost, dockerRemoteApiVersion);
            else
                client = ExperimentDockerClient.create(dockerHost);
            return ExperimentDistributor.create(client, builder);
        }
    }
}