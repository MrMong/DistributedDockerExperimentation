using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DistributedExperimentation.Investigator.Application
{
    public class ExperimentDockerClient
    {
        private DockerClient client;

        private ExperimentDockerClient(Uri host, System.Version version) 
        {
            bool isOk = ((host != null) && (version != null));
            if (isOk) {
                this.client = new DockerClientConfiguration(host).CreateClient(version);
            } else {
                throw new ArgumentException("Arguments 'host' and 'version' must be not null.");
            }
        }

        private ExperimentDockerClient(Uri host)
        {
            bool isOk = (host != null);
            if (isOk) {
                this.client = new DockerClientConfiguration(host).CreateClient();
            } else {
                throw new ArgumentException("Argument 'host' must be not null.");
            }            
        }

        public static ExperimentDockerClient create(Uri host, System.Version version)
        {
            return new ExperimentDockerClient(host, version);
        }

        public static ExperimentDockerClient create(Uri host)
        {
            return new ExperimentDockerClient(host);
        }        

        public async Task<IList<ContainerListResponse>> listAllContainersAsync(long limit) 
        {
            IList<ContainerListResponse> containers = await this.client.Containers.ListContainersAsync(
                new ContainersListParameters(){
                    Limit = limit,
                });
            return containers;
        }

        public async Task<String> createSwarmAsync(String advertiseAddr, String listenAddr)
        {
            SwarmInitParameters parameters = new SwarmInitParameters();
            parameters.AdvertiseAddr = advertiseAddr;
            parameters.ListenAddr = listenAddr;
            CancellationToken cancellationToken = new CancellationToken();
            return await this.client.Swarm.InitSwarmAsync(parameters, cancellationToken);
        }

        public async Task leaveSwarmAsync(bool isForced)
        {
            SwarmLeaveParameters parameters = new SwarmLeaveParameters();
            parameters.Force = isForced;
            CancellationToken cancellationToken = new CancellationToken();
            await this.client.Swarm.LeaveSwarmAsync(parameters, cancellationToken);
        }

        public async Task<String> inspectSwarmAsync()
        {
            SwarmInspectResponse response = await this.client.Swarm.InspectSwarmAsync();
            return response.ToString();
        }  

        public async Task<IList<KeyValuePair<string, string>>> getServiceTaskStatesAsync() 
        {
            IList<KeyValuePair<string, string>> resultTasks = new List<KeyValuePair<string, string>>();
            IList<TaskResponse> tasks = await this.client.Tasks.ListAsync();
            foreach(TaskResponse task in tasks) {
                resultTasks.Add(new KeyValuePair<string, string>(task.ServiceID, task.Status.State.ToString()));
            }
            return resultTasks;
        }

        public async Task updateNodeAsync(String hostname, String role, String availability, 
                                          ulong version, KeyValuePair<String, String> label) 
        {
            NodeUpdateParameters parameters = new NodeUpdateParameters();
            IDictionary<String,String> dictionary = new Dictionary<String, String>();
            dictionary.Add(label);
            parameters.Labels = dictionary;
            parameters.Role = role;
            parameters.Availability = availability;
            await this.client.Swarm.UpdateNodeAsync(hostname, version, parameters);
        }

        public async Task<String> createExperimentServiceAsync(String taskName, String dockerImage, 
                                                               String experimenterPath, String jsonExperiment)
        {
            ServiceCreateParameters parameters = new ServiceCreateParameters();
            parameters.Service = new ServiceSpec();
            parameters.Service.Name = taskName;
            parameters.Service.TaskTemplate = new TaskSpec();
            parameters.Service.TaskTemplate.RestartPolicy = new SwarmRestartPolicy();
            parameters.Service.TaskTemplate.RestartPolicy.Condition = "none";
            parameters.Service.Mode = new ServiceMode();
            parameters.Service.Mode.Replicated = new ReplicatedService();
            parameters.Service.Mode.Replicated.Replicas = 1;
            NetworkAttachmentConfig networkAttach1 = new NetworkAttachmentConfig();
            networkAttach1.Target = "host";
            IList<NetworkAttachmentConfig> networks = new List<NetworkAttachmentConfig>();
            networks.Add(networkAttach1);
            parameters.Service.TaskTemplate.Networks = networks;

            parameters.Service.TaskTemplate.ContainerSpec = new ContainerSpec();
            parameters.Service.TaskTemplate.ContainerSpec.Image = dockerImage;
            IList<String> containerCommand = new List<String>();
            containerCommand.Add(experimenterPath);
            containerCommand.Add("--experiment-data");
            containerCommand.Add(jsonExperiment);
            parameters.Service.TaskTemplate.ContainerSpec.Command = containerCommand;
            
            ServiceCreateResponse response = await this.client.Swarm.CreateServiceAsync(parameters);
            return response.ID;
        }

        public async Task removeExperimentServiceAsync(String serviceId) 
        {
            await this.client.Swarm.RemoveServiceAsync(serviceId);
        }
    }
}