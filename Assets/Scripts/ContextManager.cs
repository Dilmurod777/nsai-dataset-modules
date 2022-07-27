using System;
using System.Linq;
using Custom;
using Instances;
using UnityEngine;

public class ContextManager : Singleton<ContextManager>
{
	public Task CurrentTask;
	public Subtask CurrentSubtask;
	public Instruction CurrentInstruction;
	public Query CurrentQuery;

	public object Prev;
	public object Var1;
	public object Var2;

	public T GetAttribute<T>(string attributeName)
	{
		var result = attributeName.ToLower() switch
		{
			"query" => (T) (object) Instance.CurrentQuery,
			"programs" => Instance.CurrentQuery != null ? (T) (object) Instance.CurrentQuery.Programs : (T) (object) null,
			"var1" => (T) Instance.Var1,
			"var2" => (T) Instance.Var2,
			"prev" => (T) Instance.Prev,
			"root" => (T) (object) KnowledgeManager.Instance.Root,
			"current_task_id" => (T) (object) Instance.CurrentTask.TaskId,
			"current_subtask_id" => (T) (object) Instance.CurrentSubtask.SubtaskId,
			"current_instruction_order" => (T) (object) Instance.CurrentInstruction.Order,
			_ => (T) (object) null
		};

		return result;
	}

	public bool HasAttribute(string attributeName)
	{
		var attributes = new[] {"var1", "var2", "prev", "root", "query", "programs"};

		return attributes.Contains(attributeName);
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

		var figurePlainName = Helpers.GetCurrentFigurePlainName();
		var task = Instance.CurrentTask;
		var taskType = GetTaskType(task);

		var figure = GameObject.Find(figurePlainName + taskType);
		AssetManager.Instance.ResetFigure(figure);
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

	public static Constants.TaskType GetTaskType(Task task)
	{
		if (task.Title.ToLower().Contains("install"))
		{
			return Constants.TaskType.Installation;
		}

		if (task.Title.ToLower().Contains("remov"))
		{
			return Constants.TaskType.Removal;
		}

		return Constants.TaskType.Other;
	}
}