using System;
using System.Collections.Generic;
using Instances;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public GameObject uiOption;

	public enum UIOptionType
	{
		Task,
		Subtask,
		Query
	}

	public void SetUpMenus()
	{
		SetUpKnowledgeOptions(RootManager.Instance.knowledgeManager.Tasks);
		SetUpQueryOptions(RootManager.Instance.knowledgeManager.Queries);
	}

	public void UpdateUI()
	{
		var homeButton = GameObject.FindWithTag("HomeButton");
		homeButton!.GetComponent<Button>().onClick.RemoveAllListeners();
		homeButton!.GetComponent<Button>().onClick.AddListener(() => { RootManager.Instance.sceneManager.LoadScene(SceneManager.StartMenuSceneName); });
		;

		if (RootManager.Instance.contextManager.CurrentQuery != null)
		{
			var queryText = GameObject.FindWithTag("QueryText");

			if (queryText != null)
			{
				queryText.GetComponent<InputField>().text = RootManager.Instance.contextManager.CurrentQuery.Title;
			}
		}

		var knowledgeTaskUI = GameObject.FindWithTag("KnowledgeTaskUI");
		knowledgeTaskUI!.GetComponent<Image>().enabled = RootManager.Instance.contextManager.CurrentTask != null;
		knowledgeTaskUI!.transform.GetChild(0).GetComponent<Text>().text = RootManager.Instance.contextManager.CurrentTask != null
			? "Task " + RootManager.Instance.contextManager.CurrentTask.TaskId
			: "";

		var knowledgeSubtaskUI = GameObject.FindWithTag("KnowledgeSubtaskUI");
		knowledgeSubtaskUI!.GetComponent<Image>().enabled = RootManager.Instance.contextManager.CurrentSubtask != null;
		knowledgeSubtaskUI!.transform.GetChild(0).GetComponent<Text>().text = RootManager.Instance.contextManager.CurrentSubtask != null
			? "Subtask " + RootManager.Instance.contextManager.CurrentSubtask.SubtaskId
			: "";

		var knowledgeInstructionUI = GameObject.FindWithTag("KnowledgeInstructionUI");
		knowledgeInstructionUI!.GetComponent<Image>().enabled = RootManager.Instance.contextManager.CurrentInstruction != null;
		knowledgeInstructionUI!.transform.GetChild(0).GetComponent<Text>().text = RootManager.Instance.contextManager.CurrentInstruction != null
			? RootManager.Instance.contextManager.CurrentInstruction.Content
			: "";
	}

	private void SetUpKnowledgeOptions(List<Task> tasks)
	{
		var knowledgeContent = GameObject.FindWithTag("KnowledgeContent");

		if (knowledgeContent.transform.childCount == 0)
		{
			for (var i = 0; i < tasks.Count; i++)
			{
				var task = tasks[i];
				var newTaskOption = Instantiate(uiOption, knowledgeContent.transform, false);
				newTaskOption.name = task.TaskId;
				newTaskOption.GetComponent<Button>().onClick.AddListener(() => { UIOptionClick(UIOptionType.Task, new dynamic[] {task}); });

				var newTaskOptionTextComponent = newTaskOption.transform.GetChild(0).GetComponent<Text>();
				newTaskOptionTextComponent.text = "Task " + task.TaskId;

				var list = Instantiate(new GameObject(), knowledgeContent.transform, false);
				var listVerticalLayoutGroup = list.AddComponent<VerticalLayoutGroup>();
				var listContentSizeFitter = list.AddComponent<ContentSizeFitter>();

				list.name = "Subtasks-List";
				listVerticalLayoutGroup.padding.left = 50;
				listVerticalLayoutGroup.childControlWidth = false;
				listVerticalLayoutGroup.childControlHeight = false;
				listVerticalLayoutGroup.childForceExpandWidth = true;
				listVerticalLayoutGroup.childForceExpandHeight = false;

				listContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

				for (var j = 0; j < task.Subtasks.Count; j++)
				{
					var subtask = task.Subtasks[j];
					var newSubtaskOption = Instantiate(uiOption, list.transform, false);
					newSubtaskOption.name = subtask.SubtaskId;
					newSubtaskOption.GetComponent<Button>().onClick.AddListener(() => { UIOptionClick(UIOptionType.Subtask, new dynamic[] {task, subtask}); });

					var newSubtaskOptionTextComponent = newSubtaskOption.transform.GetChild(0).GetComponent<Text>();
					newSubtaskOptionTextComponent.text = "Subtask " + task.Subtasks[j].SubtaskId;
				}
			}
		}

		var knowledgeScrollRect = knowledgeContent.transform.parent.GetComponent<ScrollRect>();
		knowledgeScrollRect!.normalizedPosition = new Vector2(0, 1);
	}

	private void SetUpQueryOptions(List<Query> queries)
	{
		var queriesContent = GameObject.FindWithTag("QueriesContent");

		if (queriesContent.transform.childCount == 0)
		{
			for (var i = 0; i < queries.Count; i++)
			{
				var query = queries[i];
				var newQueryOption = Instantiate(uiOption, queriesContent.transform, false);
				newQueryOption.name = query.Filename;
				newQueryOption.GetComponent<Button>().onClick.AddListener(() => { UIOptionClick(UIOptionType.Query, new dynamic[] {query}); });

				var newQueryOptionTextComponent = newQueryOption.transform.GetChild(0).GetComponent<Text>();
				newQueryOptionTextComponent.text = query.Filename;
			}
		}

		var queryScrollRect = queriesContent.transform.parent.GetComponent<ScrollRect>();
		queryScrollRect.normalizedPosition = new Vector2(0, 1);
	}

	private void UIOptionClick(UIOptionType type, dynamic[] data)
	{
		Task task;
		Subtask subtask;
		Instruction instruction;
		Query query;

		switch (type)
		{
			case UIOptionType.Task:
				task = data[0] as Task;
				subtask = task!.Subtasks.Count > 0 ? task!.Subtasks[0] : null;
				instruction = subtask?.Instructions.Count > 0 ? subtask.Instructions[0] : null;

				RootManager.Instance.contextManager.SetCurrentTask(task);
				RootManager.Instance.contextManager.SetCurrentSubtask(subtask);
				RootManager.Instance.contextManager.SetCurrentInstruction(instruction);
				break;
			case UIOptionType.Subtask:
				task = data[0] as Task;
				subtask = data[1] as Subtask;
				instruction = subtask?.Instructions.Count > 0 ? subtask.Instructions[0] : null;
				
				RootManager.Instance.contextManager.SetCurrentTask(task);
				RootManager.Instance.contextManager.SetCurrentSubtask(subtask);
				RootManager.Instance.contextManager.SetCurrentInstruction(instruction);
				break;
			case UIOptionType.Query:
				query = data[0] as Query;
				RootManager.Instance.contextManager.SetCurrentQuery(query);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}

		RootManager.Instance.sceneManager.LoadScene(SceneManager.MainSceneName);
	}
}