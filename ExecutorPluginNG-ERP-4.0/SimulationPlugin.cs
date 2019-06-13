using DistributedExperimentation.DataModel;
using DistributedExperimentation.Experimenter.ExecutorPlugin;
using System;
using System.Collections.Generic;

namespace ExecutorPluginNGERP40
{
    public class ExecutorPlugin : IExecutorPlugin
    {
        public void execute(IExperimentSeries experimentSeries)
        {
            var experimentCollection = experimentSeries.getExperiments();
            foreach (var experiment in experimentCollection)
            {
                SolveCollection((IParameterList)experiment.getParameters());
            }
        }

        private void SolveCollection(IParameterList parameterList)
        {
            for (uint i = 0; i < parameterList.count(); i++)
            {
                var parameter = parameterList.get(i);
                if (parameter.getValue().isPrimitive())
                {
                    System.Diagnostics.Debug.WriteLine("Parameter: {0}", i);
                    System.Diagnostics.Debug.WriteLine(parameter.getValue());
                    System.Diagnostics.Debug.WriteLine(parameter.getValue().GetType());
                }
                else
                {
                    SolveCollection((IParameterList)parameter);
                }

            }
        }
    }
}
