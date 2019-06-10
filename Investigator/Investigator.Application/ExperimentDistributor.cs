using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedExperimentation.DataModel;
using DistributedExperimentation.DataModel.Parsing;
using DistributedExperimentation.DataModel.Parsing.Implementation;

namespace DistributedExperimentation.Investigator.Application
{
    public class ExperimentDistributor
    {
        private ExperimentDockerClient client;
        private IExperimentSeriesBuilder builder;

        private ExperimentDistributor(ExperimentDockerClient client, 
                                    IExperimentSeriesBuilder builder)
        {
            bool isOk = ((client != null) && (builder != null));
            if (isOk) {
                this.client = client;
                this.builder = builder;
            } else {
                throw new ArgumentException("Arguments 'client' and " +
                                            "'jbuilder' must be not null.");
            }
        }

        public static ExperimentDistributor create(ExperimentDockerClient client,
                                          IExperimentSeriesBuilder builder) 
        {
            return new ExperimentDistributor(client, builder);
        }

        public async Task distributeExperimentSeries(IExperimentSeries eSeries, 
                                                     String dockerImage, 
                                                     String executionPath) 
        {
            bool isOk = ((eSeries != null) && (dockerImage != null) && (executionPath != null));
            if (isOk) {
                IList<Task<KeyValuePair<String, String>>> tasks = new List<Task<KeyValuePair<String, String>>>();
                Console.WriteLine("Investigator: Start distributing experiment tasks of " +
                                  "series of experiment with id " + eSeries.getId());
                foreach(IExperiment experiment in eSeries.getExperiments())
                {
                    tasks.Add(this.distributeExperimentAsync(eSeries.getId(), 
                                                           eSeries.getName(), 
                                                           eSeries.getDescription(),
                                                           eSeries.getExperimentSoftware(), 
                                                           experiment, dockerImage, executionPath));
                }  
                await startRemovePolling(tasks.Select(x => x.Result).ToList());
                Console.WriteLine("Investigator: Experiment tasks of the series of experiment " +
                                  "with id " + eSeries.getId() + " were finished");              
            } else {
                throw new ArgumentException("Arguments 'eSeries','dockerImage' and " +
                                            "'executionPath' must be not null.");
            }
        }

        private async Task startRemovePolling(IList<KeyValuePair<String, String>> currentExperimentServiceIds)
        {
            IList<KeyValuePair<String, String>> copy = currentExperimentServiceIds.ToList();
            IList<String> sIds = copy.Select(x => x.Key).ToList();
            while (sIds.Count > 0) {
                IList<KeyValuePair<string, string>> taskStates = client.getServiceTaskStatesAsync().Result;
                foreach(KeyValuePair<string, string> t in taskStates) {
                    bool isContained = (sIds.Contains(t.Key));
                    bool isExpired = ((String.Compare(t.Value, "complete", true) == 0) ||
                                     (String.Compare(t.Value, "failed", true) == 0) ||
                                     (String.Compare(t.Value, "shutdown", true) == 0) ||
                                     (String.Compare(t.Value, "rejected", true) == 0) ||
                                     (String.Compare(t.Value, "orphaned", true) == 0) ||
                                     (String.Compare(t.Value, "remove", true) == 0));
                    if (isContained && isExpired) {
                        KeyValuePair<string, string> expTask = copy.Where(x => 
                                    String.Compare(x.Key, t.Key) == 0).First();
                        Console.WriteLine("Investigator: Experiment task with id\n\t        " +
                                          expTask.Value + "\n\t      has been terminated " +
                                          "with state " + t.Value);
                        Console.WriteLine("Investigator: Remove experiment service " +
                                          "task with id\n\t        " + expTask.Value);
                        await this.client.removeExperimentServiceAsync(t.Key);
                        Console.WriteLine("Investigator: Experiment service task " +
                                          "with id\n\t        " + expTask.Value +
                                          "\n\t      was removed");                    
                        sIds.Remove(t.Key);
                    }
                }
                Task.Delay(3000).Wait();
            }
        }

        private async Task<KeyValuePair<String, String>> distributeExperimentAsync(String seriesId, 
                                                                                 String seriesName, 
                                                                                 String seriesDescription, 
                                                                                 String seriesSoftwarename, 
                                                                                 IExperiment experiment, 
                                                                                 String dockerImage,
                                                                                 String executionPath)
        {
            String path = executionPath;
            builder.setId(seriesId);
            builder.setName(seriesName);
            builder.setDescription(seriesDescription);
            builder.setSoftwareName(seriesSoftwarename);
            builder.add(experiment);
            String json = (String)builder.build();
            builder.reset();
            String experimentIdentifier = System.Guid.NewGuid().ToString() + "----" +
                                          seriesId + "-series--" + experiment.getId() + "-experiment";
            Console.WriteLine("Investigator: Create experiment service task " +
                              "with generated id\n\t        "+ experimentIdentifier);
            String serviceId = await this.client.createExperimentServiceAsync(experimentIdentifier, 
                                                                              dockerImage, path, json);
            Console.WriteLine("Investigator: Experiment service task of id\n\t        " +
                              experimentIdentifier + "\n\t      has been started\n\t      " +
                              "The experiment started inside the docker service with id " + serviceId);                                                                         
            return new KeyValuePair<string, string>(serviceId, experimentIdentifier);
        }
    }
}