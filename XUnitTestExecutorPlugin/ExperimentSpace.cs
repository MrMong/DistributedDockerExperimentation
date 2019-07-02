using System;
using System.Collections.Generic;
using System.Linq;

namespace XUnitTestExecutorPlugin
{
    public enum Gateway
    {
        AND,
        XOR
    }


    public interface INode
    {
        Guid Id { get; }
        bool IsPropertie { get; }
        bool HasActiveNodes { get; set; }
        object Value { get; }
        Gateway Gateway { get; }
        void GetCandidate(List<IProperty> resultCollection);
    }

    public class Node : INode
    {
        public Node(object value, bool hasActiveNodes = false, Gateway gate = Gateway.AND)
        {

            HasActiveNodes = hasActiveNodes;
            Gateway = gate;
            Value = value;
            Id = Guid.NewGuid();
        }
        public Guid Id { get; }
        [ThreadStatic] private static Guid ControllNode = Guid.Empty;
        public bool IsPropertie => Value.GetType() == typeof(List<IProperty>); 
        public object Value { get; set; }
        public bool HasActiveNodes { get; set; }
        public Gateway Gateway { get; }
        public void GetCandidate(List<IProperty> resultCollection)
        {
            HasActiveNodes = true;
            if (IsPropertie) {
                resultCollection.Add(GetPropertieValue(this));
                return;
            }

            //else
            if (Gateway.AND == this.Gateway)
            {
                foreach (var item in (ICollection<INode>)Value)
                {
                    item.GetCandidate(resultCollection);
                }
            } else // Gateway.XOR
            {

            }
            return;
        }

        private IProperty GetPropertieValue(INode node)
        {
            var steps = (List<IProperty>)node.Value;
            var currentActiveValue = steps.FirstOrDefault(x => x.IsActive == true);
            if (currentActiveValue == null) { 
                currentActiveValue = steps[0];
                currentActiveValue.IsActive = true;
            }

            if (ControllNode == Guid.Empty)
                ControllNode = this.Id;

            if (ControllNode != this.Id)
            {
                return currentActiveValue;
            }
            //else
            var currentIndex = steps.IndexOf(currentActiveValue);
            currentActiveValue.IsActive = false;
            if (currentIndex != (steps.Count - 1))
            {
                steps[currentIndex + 1].IsActive = true;
            }
            else {
                steps[0].IsActive = true;
                ControllNode = Guid.Empty;
            }
            return currentActiveValue;
        }
    }

    public class Propertie : IProperty
    {
        public bool IsActive { get; set; }
        public string PropertieName { get; set; }
        public object Value { get; set; } 
    }

    public interface IProperty
    {
        bool IsActive { get; set; }
        string PropertieName { get; }
        object Value { get; }
    }
}
