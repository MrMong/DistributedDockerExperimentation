using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using DistributedExperimentation.DataModel;
using DistributedExperimentation.Experimenter.ExecutorPlugin;

namespace DistributedExperimentation.Experimenter.Application
{
    public class ExecutorPluginProxy : IExecutorPlugin
    {
        private String pluginFilePath;
        private IExecutorPlugin executorPlugin;

        private ExecutorPluginProxy(String pluginFilePath) {
            bool isOk = ((!String.IsNullOrEmpty(pluginFilePath)) &&  
                        (File.Exists(pluginFilePath)));
            if (isOk) {
                this.pluginFilePath = pluginFilePath;
                this.executorPlugin = executorPlugin = loadPlugin(pluginFilePath);
            } else {
                throw new ArgumentException("The given arguments are not valid.\n" +
                                            "pluginFilePathArg must be a valid non empty "+
                                            "string and a valid file path to an plugin file");
            }
        }

        public static ExecutorPluginProxy create(String pluginFilePath)
        {
            return new ExecutorPluginProxy(pluginFilePath);
        }

        public void execute(IExperimentSeries experimentSeries) 
        {
            this.executorPlugin.execute(experimentSeries);
        }

        public String getPluginDirPath() 
        {
            return this.pluginFilePath;
        }

        private IExecutorPlugin loadPlugin(String pluginFilePathArg) 
        {
            IExecutorPlugin loadedPlugin = null;
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginFilePathArg); 
            if(assembly != null) { 
                Type pluginType = typeof(IExecutorPlugin); 
                Type[] types = assembly.GetTypes(); 
                foreach(Type type in types) 
                { 
                    if(!type.IsInterface && !type.IsAbstract) { 
                        if(type.GetInterface(pluginType.FullName) != null) { 
                            loadedPlugin = (IExecutorPlugin)Activator.CreateInstance(type); 
                        } 
                    } 
                } 
            }
            return loadedPlugin;
        }
    }
}