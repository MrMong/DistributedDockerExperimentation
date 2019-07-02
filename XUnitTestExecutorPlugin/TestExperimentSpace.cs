
using System.Collections.Generic;
using Xunit;

namespace XUnitTestExecutorPlugin
{
    public class TestExperimentSpace
    {
        private INode SingleNodes = new Node(new List<IProperty> { new Propertie { PropertieName = "Diversity", Value = 0 },
                                                                  new Propertie { PropertieName = "Diversity", Value = 10 },
                                                                  new Propertie { PropertieName = "Diversity", Value = 20 },}
        );

        private INode AndNodes = new Node(
            new List<INode> {
                new Node(new List<IProperty> { new Propertie { PropertieName = "Diversity", Value = 0 },
                                               new Propertie { PropertieName = "Diversity", Value = 10 },}),
                new Node(new List<IProperty> { new Propertie { PropertieName = "Length", Value = 1000 },
                                               new Propertie { PropertieName = "Length", Value = 1500 },})
            }, true, Gateway.AND
        );


        [Fact]
        public void SinglePropertieList()
        {
            var test = new List<List<IProperty>>();
            var startnode = true;
            

            while (startnode)
            {
                var candidate = new List<IProperty>();
                SingleNodes.GetCandidate(candidate);
                test.Add(candidate);
            }

            Assert.True(test.Count == 3);
        }

        [Fact]
        public void AndConnectedPropertieList()
        {
            var test = new List<List<IProperty>>();
            var startnode = true;


            while (AndNodes.HasActiveNodes)
            {
                var candidate = new List<IProperty>();
                AndNodes.GetCandidate(candidate);
                test.Add(candidate);
            }

            Assert.True(test.Count == 3);
        }
    }
}
