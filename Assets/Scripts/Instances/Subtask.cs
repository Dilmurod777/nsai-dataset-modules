using System.Collections.Generic;
using Custom;

namespace Instances
{
	public class Subtask
	{
		public string SubtaskId;
		public string Content;
		public string Figure;
		public List<Instruction> Instructions;
		public bool isCompleted;

		public Subtask(string subtaskId, string content, List<Instruction> instructions, string figure)
		{
			SubtaskId = subtaskId;
			Content = content;
			Instructions = instructions;
			Figure = figure;
			isCompleted = false;
		}

		public void SetIsCompleted(bool state)
		{
			isCompleted = state;
		}
	}
}