using System.Collections.Generic;

namespace Instances
{
    public class Instruction
    {
        public string Order;
        public string Content;
        public List<Action> Actions;

        public Instruction(string order, string content, List<Action> actions)
        {
            Order = order;
            Content = content;
            Actions = actions;
        }
    }
}