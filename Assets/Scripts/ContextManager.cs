using System.Linq;
using Custom;
using Instances;

public class ContextManager : Singleton<ContextManager>
{
	public Task CurrentTask;
	public Subtask CurrentSubtask;
	public Instruction CurrentInstruction;
	public Query CurrentQuery;

	public dynamic Prev;
	public object Var1;
	public object Var2;
	public string[] Programs;

	public enum TaskType
	{
		Installation,
		Removal,
		Other
	}

	public dynamic GetAttribute(string name)
	{
		return name switch
		{
			"query" => Instance.CurrentQuery,
			"programs" => Instance.Programs,
			"var1" => Instance.Var1,
			"var2" => Instance.Var2,
			"prev" => Instance.Prev,
			"root" => KnowledgeManager.Instance.Tasks,
			"CurrentTaskID" => Instance.CurrentTask.TaskId,
			"CurrentSubtaskID" => Instance.CurrentSubtask.SubtaskId,
			"CurrentInstructionOrder" => Instance.CurrentInstruction.Order,
			_ => null
		};
	}

	public bool HasAttribute(string name)
	{
		var attributes = new[] {"var1", "var2", "prev", "root", "query", "programs"};

		return attributes.Contains(name);
	}

	public void SetCurrentTask(Task task)
	{
		CurrentTask = task;
		CurrentSubtask = CurrentTask != null && CurrentTask.Subtasks.Count > 0 ? CurrentTask.Subtasks[0] : null;
		CurrentInstruction = CurrentSubtask != null && CurrentSubtask.Instructions.Count > 0 ? CurrentSubtask.Instructions[0] : null;
	}

	public void SetCurrentTask(string taskId)
	{
		CurrentTask = KnowledgeManager.Instance.Tasks.First(task => task.TaskId == taskId);
		CurrentSubtask = CurrentTask != null && CurrentTask.Subtasks.Count > 0 ? CurrentTask.Subtasks[0] : null;
		CurrentInstruction = CurrentSubtask != null && CurrentSubtask.Instructions.Count > 0 ? CurrentSubtask.Instructions[0] : null;
	}

	public void SetCurrentSubtask(Subtask subtask)
	{
		CurrentSubtask = subtask;
		CurrentInstruction = CurrentSubtask != null && CurrentSubtask.Instructions.Count > 0 ? CurrentSubtask.Instructions[0] : null;
		AssetManager.Instance.UpdateAssets();
	}

	public void SetCurrentSubtask(string subtaskId)
	{
		CurrentSubtask = Instance.CurrentTask!.Subtasks.First(subtask => subtask.SubtaskId == subtaskId);
		CurrentInstruction = CurrentSubtask != null && CurrentSubtask.Instructions.Count > 0 ? CurrentSubtask.Instructions[0] : null;
		AssetManager.Instance.UpdateAssets();
	}

	public void SetCurrentInstruction(Instruction instruction)
	{
		CurrentInstruction = instruction;
	}

	public void SetCurrentInstruction(string instructionOrder)
	{
		CurrentInstruction = Instance.CurrentSubtask!.Instructions.First(instr => instr.Order == instructionOrder);
	}

	public void SetCurrentQuery(Query query)
	{
		CurrentQuery = query;
		SetCurrentTask(query.TaskId);
		SetCurrentSubtask(query.SubtaskId);
		SetCurrentInstruction(query.InstructionOrder);
	}

	public void ResetCurrentTask()
	{
		CurrentTask = null;
		CurrentSubtask = null;
		CurrentInstruction = null;
		CurrentQuery = null;
	}

	public void ResetCurrentSubtask()
	{
		CurrentSubtask = null;
		CurrentInstruction = null;
		CurrentQuery = null;
	}

	public static TaskType GetTaskType(Task task)
	{
		if (task.Title.ToLower().Contains("install"))
		{
			return TaskType.Installation;
		}

		if (task.Title.ToLower().Contains("remov"))
		{
			return TaskType.Removal;
		}

		return TaskType.Other;
	}
}