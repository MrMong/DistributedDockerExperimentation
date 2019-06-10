using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DistributedExperimentation.Experimenter.Application;

namespace DistributedExperimentation.Experimenter.UI
{
    class ApplicationMain
    {
        static void Main(string[] args)
        {
            try {
                Dictionary<string, string> parsedArgs = parseArguments(args);
                String pluginPath = getPluginFilePath("ExecutorPlugin", Directory.GetCurrentDirectory());
                ExecutorPluginProxy proxy = ExecutorPluginProxy.create(pluginPath);
                Application.Experimenter experimenter = Application.Experimenter.create();
                experimenter.executeExperimentation(parsedArgs["--experiment-data"], proxy);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        static Dictionary<string, string> parseArguments(string[] args)
        {
            try {
                if (args.Length != 1)
                    throw new Exception();
                Dictionary<string, string> parsedArgs = new Dictionary<string, string>();
                foreach(String arg in args) {
                    String key = arg.Substring(0, arg.IndexOf(' ')).Trim();
                    String val = arg.Substring(arg.IndexOf(' ')).Trim();
                    if (val.First().Equals('\''))
                        val = val.Trim('\'');
                    else if (val.First().Equals('\"'))
                        val = val.Trim('"');
                    parsedArgs.Add(key, val);
                }
                if (!parsedArgs.ContainsKey("--experiment-data"))
                    throw new Exception();
                return parsedArgs;           
            } catch (Exception ex) {
                throw new System.ArgumentException("Given arguments are not correct.\n"+
                                                   "Should be [ --experiment-data jsonExperimentSeriesString ]\n"+
                                                   "Given arguments: [ "+ String.Join(' ', args) +" ]", ex);
            }
        }

        static String getPluginFilePath(String pluginFilePrefix, String pluginDirPath) {
            bool isOk = ((!String.IsNullOrEmpty(pluginFilePrefix)) &&
                        (!String.IsNullOrEmpty(pluginDirPath)));
            if (!isOk)
                throw new ArgumentException("One or more given arguments are not valid.\n" +
                                            "pluginFilePrefix must be a valid non empty string\n"+
                                            "pluginDirPath must be a valid non empty string");
            string[] pluginFiles = Directory.GetFiles(pluginDirPath, pluginFilePrefix + "*.dll");
            if (pluginFiles.Length < 1)
                throw new FileNotFoundException("Could not found a plugin file with the given filename " +
                                                "prefix '" + pluginFilePrefix + "' in the given directory " + 
                                                pluginDirPath);
            return pluginFiles[0];
        }
    }
}
