using System;
using System.Linq;
using Custom;
using Instances;

public class ContextManager : Singleton<ContextManager>
{
	public Task CurrentTask;
	public Subtask CurrentSubtask;
	public Instruction CurrentInstruction;
	public Query CurrentQuery;

	public object Prev;
	public object Var1;
	public object Var2;

	public enum TaskType
	{
		Installation,
		Removal,
		Other
	}

	public T GetAttribute<T>(string attributeName)
	{
		return attributeName switch
		{
			"query" => (T) (object) Instance.CurrentQuery,
			"programs" => Instance.CurrentQuery != null ? (T) (object) Instance.CurrentQuery.Programs : (T) (object) null,
			"var1" => (T) Instance.Var1,
			"var2" => (T) Instance.Var2,
			"prev" => (T) Instance.Prev,
			"root" => (T) (object) KnowledgeManager.Instance.Tasks,
			"CurrentTaskID" => (T) (object) Instance.CurrentTask.TaskId,
			"CurrentSubtaskID" => (T) (object) Instance.CurrentSubtask.SubtaskId,
			"CurrentInstructionOrder" => (T) (object) Instance.CurrentInstruction.Order,
			_ => (T) (object) null
		};
	}

	public bool HasAttribute(string attributeName)
	{
		var attributes = new[] {"var1", "var2", "prev", "root", "query", "programs"};

		return attributes.Contains(attributeName);
	}

	public Type GetAttributeType(string attributeName)
	{
		return attributeName switch
		{
			"query" => Instance.CurrentQuery.GetType(),
			"programs" => Instance.CurrentQuery?.Programs.GetType(),
			"var1" => Instance.Var1.GetType(),
			"var2" => Instance.Var2.GetType(),
			"prev" => Instance.Prev.GetType(),
			"root" => KnowledgeManager.Instance.Tasks.GetType(),
			"CurrentTaskID" => Instance.CurrentTask.TaskId.GetType(),
			"CurrentSubtaskID" => Instance.CurrentSubtask.SubtaskId.GetType(),
			"CurrentInstructionOrder" => Instance.CurrentInstruction.Order.GetType(),
			_ => null
		};
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