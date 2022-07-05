using System.Collections.Generic;
// ReSharper disable ArrangeNamespaceBody

namespace Instances
{
    public class Action
    {
        public string Operation;
        public List<string> Components;

        public Action(string operation, List<string> components)
        {
            Operation = operation;
            Components = components;
        }
    }
}