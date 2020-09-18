# Distributed Experimentation with Docker

This distributed application allows to define a set of computer based experiments, to separate them, to distribute them to several computers in a network, to execute them and to automatically combine the results in a central location.

<a href="https://user-images.githubusercontent.com/9590880/93592402-67408d00-f9b2-11ea-8448-c4b492a3d2ec.png"><img src="https://user-images.githubusercontent.com/9590880/93592402-67408d00-f9b2-11ea-8448-c4b492a3d2ec.png" width="20%"></a>

To encapsulate different and individual experiment software and there enviroments, the application uses docker images, which must be provided from the user.

### Requirements

* Cluster network of minimum 2 docker stations, which use Docker Swarm Mode as middleware, is preconditioned.
* These stations must have:
	* minimum C# .NET Core 3.1
	* minimum Docker Engine 18.09.9 (with Docker Swarm Mode)

### Setup

* Implement and provide an custom plugin for the subsystem Experimenter, which instructs the Experimenter how to interact with your experiment software.
* Create and provide an executable docker image, which contain all your experiment software and the subsystem Experimenter with your implemented plugin. This image represents your full experiment enviroments with all its dependencies and will be distributed and executed in the cluster.
* Take the subsystem Investigator to that docker station, which takes on the role of manager.
* Create an definition file, which contains a set of computer based experiments corresponding your experiment software.

### Usage