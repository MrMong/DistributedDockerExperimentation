# Distributed Experimentation with Docker

This distributed application allows to define a set of computer based experiments, to separate them, to distribute them to several computers in a cluster, to execute them and to automatically combine the results in a central location.

### Architecture and how it works

<a href="https://user-images.githubusercontent.com/9590880/93664430-4ab65a80-fa6f-11ea-9c22-9e5a6cbf7205.png"><img src="https://user-images.githubusercontent.com/9590880/93664430-4ab65a80-fa6f-11ea-9c22-9e5a6cbf7205.png" width="50%"></a>

As can be seen from figure above, the application consists of 3 parts: Investigator, Experimenter and DataModel. The Investigator subsystem takes on the role of manager and control the distribution and monitoring about the experiment tasks, which shall be performed in the cluster. It supplied a UI to initiate a execution of user-defined experiments. Single separated experiment tasks are processed by subsystem Experimenter as worker. It parametrized the experiment software according to the user-defined experiment, execute them and save the result in a central storage (for example in a database). The component DataModel contains data formats, parser for de- and serialization and validation logic.

To encapsulate different and individual experiment software and there enviroments the application uses docker images. This image(s) must be created and configured according to user requirements and must be retrievable from an local or global docker registry. Due to the applied middleware Docker Swarm Mode, thus docker image will be performed as service in the cluster. It gets down when finished or aborted its experiment task (triggered by termination of Experimenter subsystem).

### Requirements

* Cluster network of minimum 2 docker stations, which use Docker Swarm Mode as middleware, is preconditioned:
	* The station (manager), which will be executed the Investigator subsystem, must have:
		* minimum C# .NET Core 3.1
		* minimum Docker Engine 18.09.9 (with Docker Swarm Mode)
	* All other stations (worker) must have:
		* minimum Docker Engine 18.09.9 (with Docker Swarm Mode)
	* Docker image for experimentation must have:
		* minimum C# .NET Core 3.1 (for Experimenter subsystem)
	* Retrievable local or global docker registry

### Setup

1. Implement and provide an custom plugin for the subsystem Experimenter, which instructs the Experimenter how to interact with your experiment software.
1. Create and provide an executable docker image, which contain all your experiment software and the subsystem Experimenter with your implemented plugin. This image represents your full experiment enviroments with all its dependencies and will be distributed and executed in the cluster.
1. Take the subsystem Investigator to that docker station, which takes on the role of manager.

### Usage

Create an definition json file, which contains a set of computer based experiments corresponding your needs and your experiment software. Simple example is shown following:
```json
{
"series_id": "1",
"name": "Wind flow investigation",
"description": "Investigate the behavior of foo by different wind flows.",
"experiment_software": "SimulationSoft 3000",
"experiments": [
{
    "experiment_id": "1",
    "name": "Worker 1",
    "parametercollection": {
        "collection_id": "1",
        "parameters": [
        {
            "parameter_id": "1",
            "name": "wind_speed_level",
            "description": "Set speed of wind.",
            "is_primitive": true,
            "value_type": "integer",
            "value": 1
        }  
        {
            "parameter_id": "2",
            "name": "rainfall",
            "description": "Set wind with or without rainfall.",
            "is_primitive": true,
            "value_type": "boolean",
            "value": true
        }                                                                                          
        ]
    }
}                                                                                                                                                                                                                                                   
]
}
```
Initiate a execution of your defined experiments by pass
&nbsp;&nbsp;&nbsp;&nbsp;\- the path of your definition file and
&nbsp;&nbsp;&nbsp;&nbsp;\- a unique identifier of your docker image
the UI of Investigator subsystem.

After successful initiation status information will be given in terminal during the execution of experiments tasks.