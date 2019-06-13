using System;
using Xunit;
using DistributedExperimentation.Experimenter.UI;

namespace XUnitTestExecutorPlugin
{
    public class TestEpermimenterCLI
    {
        [Fact]
        public async void TestExperimentator()
        {
            ApplicationMain.Main(new string[] { "--experiment-data " + experimentJson
                                                , @"--pluginPath C:\Users\mtko\source\repos\DistributedExperimentation\ExecutorPluginNG-ERP-4.0\bin\Debug\netstandard2.0" });

            
        }

        private readonly string experimentJson =
          @"{ ""series_id"": ""1"",
              ""name"": ""Additionsreihen"",
              ""description"": ""Addition verschiedener Zahlen."",
              ""experiment_software"": ""Simple-Adder-DB"",
              ""experiments"": [
              {
                  ""experiment_id"": ""1"",
                  ""name"": ""Erste Addition"",
                  ""parametercollection"": {
                      ""collection_id"": ""1"",
                      ""parameters"": [ {
                          ""parameter_id"": ""1"",
                          ""name"": ""SimulationId"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 1
                          },
                          {
                            ""parameter_id"": ""2"",
                            ""name"": ""OrderAmount"",
                            ""description"": ""OrderAmount"",
                            ""is_primitive"": true,
                            ""value_type"": ""integer"",
                            ""value"": 550
                          },
                          {
                            ""parameter_id"": ""3"",
                            ""name"": ""InterArrivalRate"",
                            ""description"": ""Interarival Rate for Orders"",
                            ""is_primitive"": true,
                            ""value_type"": ""real"",
                            ""value"": 0.0275
                          },
                          {
                            ""parameter_id"": ""4"",
                            ""name"": ""EstimatedThroughPut"",
                            ""description"": ""Esitmated phrough put time for End Products"",
                            ""is_primitive"": true,
                            ""value_type"": ""integer"",
                            ""value"": 800
                          },
                        {        
                            ""parameter_id"": ""5"",
                            ""name"": ""Save_to_Database"",
                            ""description"": ""Save Simulation Results"",
                            ""is_primitive"": true,
                            ""value_type"": ""boolean"",
                            ""value"": true
                        },    
                        {        
                            ""parameter_id"": ""6"",
                            ""name"": ""CmdArgs"",
                            ""description"": ""CommandlineArgs"",
                            ""is_primitive"": true,
                            ""value_type"": ""characterstring"",
                            ""value"": ""1""
                        }    
                   ]
                  }
              }
              ]
            }";

    }
}
