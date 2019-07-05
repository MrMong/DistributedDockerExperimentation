using System;
using Xunit;
using DistributedExperimentation.Experimenter.UI;
using System.Threading.Tasks;

namespace XUnitTestExecutorPlugin
{
    public class TestEpermimenterCLI
    {
        [Fact]
        public async void TestExperimentator()
        {
            await Task.Run(() =>
            {
                ApplicationMain.Main(new string[] { "--experiment-data "
                                                    , experimentJson
                                                    , @"--plugin-path " 
                                                    , pathToExe});
            });
        }

        private readonly string pathToExe = @"C:\Users\mtko\source\repos\DistributedExperimentation\ExecutorPluginNG-ERP-4.0\bin\Release\netcoreapp2.2\ExecutorPluginNG-ERP-4.0.dll";

        private readonly string experimentJson =
          @"{ ""series_id"": ""1"",
              ""name"": ""SSOP Simulation"",
              ""description"": ""Self Organized Production planning run."",
              ""experiment_software"": ""First Run For debugging purpose."",
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
                          },{
                          ""parameter_id"": ""2"",
                          ""name"": ""SimulationNumber"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 1
                          },{
                          ""parameter_id"": ""3"",
                          ""name"": ""SimulationKind"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""characterstring"",
                          ""value"": ""Decentral""
                          },{
                          ""parameter_id"": ""4"",
                          ""name"": ""OrderQuantity"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 550
                          },{
                          ""parameter_id"": ""5"",
                          ""name"": ""OrderArrivalRate"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""real"",
                          ""value"": 0.0275
                          },{
                          ""parameter_id"": ""6"",
                          ""name"": ""EstimatedThroughPut"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 800
                          },{
                          ""parameter_id"": ""7"",
                          ""name"": ""KpiTimeSpan"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 480
                          },{
                          ""parameter_id"": ""7"",
                          ""name"": ""Seed"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 1337
                          },{
                          ""parameter_id"": ""8"",
                          ""name"": ""SimulationEnd"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 20160
                          },{
                          ""parameter_id"": ""9"",
                          ""name"": ""SettlingStart"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""integer"",
                          ""value"": 2880
                          },{
                          ""parameter_id"": ""10"",
                          ""name"": ""WorkTimeDeviation"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""real"",
                          ""value"": 0.2
                          },{
                          ""parameter_id"": ""10"",
                          ""name"": ""DebugAgents"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""boolean"",
                          ""value"": false
                          },{
                          ""parameter_id"": ""10"",
                          ""name"": ""DebugSystem"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""boolean"",
                          ""value"": false
                          },{
                          ""parameter_id"": ""10"",
                          ""name"": ""SaveToDB"",
                          ""description"": ""Simulation id to obtain further Simulation details"",
                          ""is_primitive"": true,
                          ""value_type"": ""boolean"",
                          ""value"": false
                          },{        
                            ""parameter_id"": ""11"",
                            ""name"": ""DBConnectionString"",
                            ""description"": ""Connection String for Result Context"",
                            ""is_primitive"": true,
                            ""value_type"": ""characterstring"",
                            ""value"": ""Server=(localdb)\\mssqllocaldb;Database=Master40Results;Trusted_Connection=True;MultipleActiveResultSets=true""
                          }    
                   ]
                  }
              }
              ]
            }";
    }
}
