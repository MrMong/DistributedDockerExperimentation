using DistributedExperimentation.DataModel;
using DistributedExperimentation.Experimenter.ExecutorPlugin;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System;

namespace ExecutorPluginNGERP40
{
    public class ExecutorPlugin : IExecutorPlugin
    {

        /// <summary>
        /// Execute The Experiment within the Docker Container
        /// </summary>
        /// <param name="experimentSeries">Returns a list with always 1 IParameterList</param>
        public void execute(IExperimentSeries experimentSeries)
        {
            var experiment = experimentSeries.getExperiments()[0];
            var properties = new Dictionary<string, object>();
            var argString = "";
            string relativePathToProgram = @"./../NgERP40/Master40.Simulation";
            //string relativePathToProgram = @".\..\Master40.Simulation\Master40.Simulation";
            string workDir = Directory.GetCurrentDirectory();
            SolveCollection((IParameterList)experiment.getParameters(), properties);

            foreach (var item in properties)
            {
                argString = argString + " " + item.Key + " " + item.Value.ToString();
                Console.WriteLine(@"{0}: {1}", item.Key, item.Value.ToString());
            }

            // initialize process and execute them40
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = workDir;
            startInfo.FileName = relativePathToProgram;
            startInfo.Arguments = argString.Trim();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;


            Process process = new Process();
            process.OutputDataReceived += RedirectOutputToConsole("Standard: ");

            process.ErrorDataReceived += RedirectOutputToConsole("Error: ");

            process.StartInfo = startInfo;
            Console.WriteLine("Simulation SSOP:");
            Console.WriteLine("-------------------------------------------------");
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();
            Console.WriteLine("-------------------------------------------------");
        }

        /// <summary>
        /// redirect Processoutput to local Console during the Execution.
        /// </summary>
        /// <param name="outputprefix">is applied in front of the Written Line.
        /// i.e. > "Error: 37a - command not found!" </param>
        /// <returns></returns>
        private static DataReceivedEventHandler RedirectOutputToConsole(string outputprefix)
        {
            return new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(outputprefix +  e.Data);
                }
            });
        }

        private void SolveCollection(IParameterList parameterList, Dictionary<string, object> targetList)
        {
           
            for (uint i = 0; i < parameterList.count(); i++)
            {
                var parameter = parameterList.get(i);
                if (parameter.getValue().isPrimitive())
                {
                    var p = parameter.getValue() as IPrimitiveValue;
                    targetList.Add(parameter.getName(), p.getRawValue());
                }
                else
                {
                    SolveCollection((IParameterList)parameter, targetList);
                }

            }
        }


    }
}
