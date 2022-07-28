using System;
using System.Collections.Generic;
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

		var previousSubtasks = new List<Subtask>();

		if (Instance.CurrentTask != null)
		{
			foreach (var s in Instance.CurrentTask.Subtasks)
			{
				if (s.SubtaskId == subtask.SubtaskId)
				{
					break;
				}

				previousSubtasks.Add(subtask);
			}
		}

		Debug.Log(previousSubtasks.Count);

		CurrentInstruction = CurrentSubtask != null && CurrentSubtask.Instructions.Count > 0 ? CurrentSubtask.Instructions[0] : null;
		AssetManager.Instance.UpdateAssets();
	}

	public void SetCurrentSubtask(string subtaskId)
	{
		var previousSubtasks = new List<Subtask>();

		if (Instance.CurrentTask != null)
		{
			foreach (var subtask in Instance.CurrentTask.Subtasks)
			{
				if (subtask.SubtaskId == subtaskId)
				{
					CurrentSubtask = subtask;
					break;
				}

				previousSubtasks.Add(subtask);
			}
		}

		CurrentInstruction = CurrentSubtask != null && CurrentSubtask.Instructions.Count > 0 ? CurrentSubtask.Instructions[0] : null;
		AssetManager.Instance.UpdateAssets();

		var figurePlanName = Helpers.GetCurrentFigurePlainName();
		var task = Instance.CurrentTask;
		var taskType = GetTaskType(task);
		var figure = GameObject.Find(figurePlanName + taskType);

		var core = figure.transform.GetComponentsInChildren<Transform>().First(child =>
		{
			var objectMeta = child.GetComponent<ObjectMeta>();

			return objectMeta && objectMeta.isCoreInFigure;
		});

		Debug.Log("Prev Subtasks Count: " + previousSubtasks.Count);
		foreach (var prevSubtask in previousSubtasks)
		{
			Debug.Log("Prev Subtask: " + prevSubtask.SubtaskId);
			var instructions = prevSubtask.Instructions;

			foreach (var instruction in instructions)
			{
				if (instruction.Actions == null || instruction.Actions.Count == 0) continue;

				foreach (var action in instruction.Actions)
				{
					var attachingObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, action.Components[0]);
					var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, "core");
					var ifmAttachingObj =
						Helpers.FindObjectInFigure(taskType == Constants.TaskType.Installation ? Constants.FigureType.Ifm : Constants.FigureType.Scattered, "core");
					var ifmReferenceObj =
						Helpers.FindObjectInFigure(taskType == Constants.TaskType.Installation ? Constants.FigureType.Ifm : Constants.FigureType.Scattered, "core");

					if (attachingObj.transform.childCount > 0)
					{
						// PrimitiveManager.Instance.GetAttachPrimitivesForChildren(attachingObj, referenceObj);
					}
					else
					{
						var diff = ifmAttachingObj.transform.position - ifmReferenceObj.transform.position;
						attachingObj.transform.position = referenceObj.transform.position + diff;

						var meshRenderer = attachingObj.GetComponent<MeshRenderer>();

						if (meshRenderer != null)
						{
							var oldMaterials = meshRenderer.materials;
							var newMaterials = new Material[oldMaterials.Length];
							for (var i = 0; i < newMaterials.Length; i++)
							{
								newMaterials[i] = new Material(AssetManager.Instance.tempMaterial);
								newMaterials[i].color = oldMaterials[i].color;
							}

							for (var i = 0; i < oldMaterials.Length; i++)
							{
								var oldColor = oldMaterials[i].color;
								var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.0f);
								newMaterials[i].color = newColor;
							}

							meshRenderer.materials = newMaterials;
						}

						// PrimitiveManager.Instance.GetAttachPrimitivesForParent(attachingObj, referenceObj);
					}
				}
			}
		}
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