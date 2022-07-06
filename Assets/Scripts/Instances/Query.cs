using System.Collections.Generic;

namespace Instances
{
	public class Query
	{
		public string Filename;
		public string Title;
		public List<string> Programs;
		public string Reply;
		public string TaskId;
		public string SubtaskId;
		public string InstructionOrder;

		public Query(string filename, string title, List<string> programs, string reply, string taskId, string subtaskId, string instructionOrder)
		{
			Filename = filename;
			Title = title;
			Programs = programs;
			Reply = reply;
			TaskId = taskId;
			SubtaskId = subtaskId;
			InstructionOrder = instructionOrder;
		}
	}
}