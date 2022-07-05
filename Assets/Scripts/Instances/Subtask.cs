using System.Collections.Generic;

namespace Instances
{
    public class Subtask
    {
        public string SubtaskId;
        public string Content;
        public string Figure;
        public List<Instruction> Instructions;

        public Subtask(string subtaskId, string content, List<Instruction> instructions, string figure)
        {
            SubtaskId = subtaskId;
            Content = content;
            Instructions = instructions;
            Figure = figure;
        }
    }
}