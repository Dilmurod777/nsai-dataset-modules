using System.Linq;
using Instances;
using UnityEngine;

public class ContextManager : MonoBehaviour
{
	public Task CurrentTask;
	public Subtask CurrentSubtask;
	public Instruction CurrentInstruction;
	public Query CurrentQuery;

	public void SetCurrentTask(Task task)
	{
		CurrentTask = task;
	}

	public void SetCurrentTask(string taskId)
	{
		CurrentTask = RootManager.Instance.knowledgeManager.Tasks.First(task => task.TaskId == taskId);
	}

	public void SetCurrentSubtask(Subtask subtask)
	{
		CurrentSubtask = subtask;
		RootManager.Instance.assetManager.UpdateAssets();
	}

	public void SetCurrentSubtask(string subtaskId)
	{
		CurrentSubtask = RootManager.Instance.contextManager.CurrentTask!.Subtasks.First(subtask => subtask.SubtaskId == subtaskId);
		RootManager.Instance.assetManager.UpdateAssets();
	}

	public void SetCurrentInstruction(Instruction instruction)
	{
		CurrentInstruction = instruction;
	}

	public void SetCurrentInstruction(string instructionOrder)
	{
		CurrentInstruction = RootManager.Instance.contextManager.CurrentSubtask!.Instructions.First(instr => instr.Order == instructionOrder);
	}

	public void SetCurrentQuery(Query query)
	{
		CurrentQuery = query;
	}
}