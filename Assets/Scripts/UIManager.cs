using System;
using System.Collections;
using System.Collections.Generic;
using Custom;
using Instances;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	public GameObject uiOption;
	public GameObject uiAction;

	public enum UIOptionType
	{
		Task,
		Subtask,
		Query
	}

	public void SetUpMenus()
	{
		SetUpKnowledgeOptions(KnowledgeManager.Instance.Tasks);
		SetUpQueryOptions(KnowledgeManager.Instance.Queries);
	}

	public void UpdateUI()
	{
		var homeButton = GameObject.FindWithTag("HomeButton");
		if (homeButton != null)
		{
			homeButton.GetComponent<Button>().onClick.RemoveAllListeners();
			homeButton.GetComponent<Button>().onClick.AddListener(() => { SceneManager.Instance.LoadScene(SceneManager.StartMenuSceneNameGlobal); });
			homeButton.GetComponent<Button>().interactable = true;
		}

		var playButton = GameObject.FindWithTag("PlayButton");
		if (playButton != null)
		{
			playButton.GetComponent<Button>().onClick.RemoveAllListeners();
			playButton.GetComponent<Button>().onClick.AddListener(() => { KnowledgeManager.Instance.ExecuteSubtask(); });
			playButton.GetComponent<Button>().interactable =
				!ContextManager.Instance.CurrentTask.isCompleted && !ContextManager.Instance.CurrentSubtask.isCompleted &&
				!KnowledgeManager.Instance.isExecutingSubtask;
		}

		var nextButton = GameObject.FindWithTag("NextButton");
		if (nextButton != null)
		{
			nextButton.GetComponent<Button>().onClick.RemoveAllListeners();
			nextButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				KnowledgeManager.GoToNextSubtask();
				Instance.UpdateUI();
			});
			nextButton.GetComponent<Button>().interactable = KnowledgeManager.HasNextSubtask();
		}

		var previousButton = GameObject.FindWithTag("PreviousButton");
		if (previousButton != null)
		{
			previousButton.GetComponent<Button>().onClick.RemoveAllListeners();
			previousButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				KnowledgeManager.GoToPreviousSubtask();
				Instance.UpdateUI();
			});
			previousButton.GetComponent<Button>().interactable = KnowledgeManager.HasPreviousSubtask();
		}

		var resetButton = GameObject.FindWithTag("ResetButton");
		if (resetButton != null)
		{
			resetButton.GetComponent<Button>().onClick.RemoveAllListeners();
			resetButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				AssetManager.Instance.ResetCurrentFigure();
				KnowledgeManager.Instance.ResetTasks();
			});
			resetButton.GetComponent<Button>().interactable = true;
		}
		
		var query = ContextManager.Instance.CurrentQuery;
		if (query != null)
		{
			var queryText = GameObject.FindWithTag("QueryText");
			queryText!.GetComponent<InputField>().text = query.Title;

			ContextManager.Instance.SetCurrentTask(query.TaskId);
			ContextManager.Instance.SetCurrentSubtask(query.SubtaskId);
			ContextManager.Instance.SetCurrentInstruction(query.InstructionOrder);
		}

		var knowledgeTaskUI = GameObject.FindWithTag("KnowledgeTaskUI");
		knowledgeTaskUI!.GetComponent<Image>().enabled = ContextManager.Instance.CurrentTask != null;
		knowledgeTaskUI!.transform.GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentTask != null
			? "Task " + ContextManager.Instance.CurrentTask.TaskId
			: "";

		var knowledgeSubtaskUI = GameObject.FindWithTag("KnowledgeSubtaskUI");
		knowledgeSubtaskUI!.GetComponent<Image>().enabled = ContextManager.Instance.CurrentSubtask != null;
		knowledgeSubtaskUI!.transform.GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentSubtask != null
			? "Subtask " + ContextManager.Instance.CurrentSubtask.SubtaskId
			: "";

		var knowledgeInstructionUI = GameObject.FindWithTag("KnowledgeInstructionUI");
		knowledgeInstructionUI!.GetComponent<Image>().enabled = ContextManager.Instance.CurrentInstruction != null;
		knowledgeInstructionUI!.transform.GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentInstruction != null
			? ContextManager.Instance.CurrentInstruction.Content
			: "";

		var knowledgeActionsUI = GameObject.FindWithTag("KnowledgeActionsUI");
		knowledgeActionsUI!.transform.GetChild(0).GetChild(0).GetComponent<Text>().enabled =
			ContextManager.Instance.CurrentInstruction != null;
		knowledgeActionsUI!.transform.GetChild(0).GetComponent<Image>().enabled = ContextManager.Instance.CurrentInstruction != null;
		var actions = ContextManager.Instance.CurrentInstruction != null
			? ContextManager.Instance.CurrentInstruction.Actions
			: null;


		var contentScrollList = knowledgeActionsUI.transform.GetChild(1).GetChild(0);
		if (actions != null)
		{
			for (var i = 0; i < contentScrollList.childCount; i++) Destroy(contentScrollList.GetChild(i).gameObject);

			foreach (var action in actions)
			{
				var newUIAction = Instantiate(uiAction, contentScrollList.transform, false);
				var capitalizedOperation = action.Operation.Substring(0, 1).ToUpper() + action.Operation.Substring(1);

				newUIAction.transform.GetChild(0).GetComponent<Text>().text = capitalizedOperation + " " + action.Components[0] + "," + action.Components[1];
			}
		}
		else
		{
			for (var i = 0; i < contentScrollList.childCount; i++) Destroy(contentScrollList.transform.GetChild(i).gameObject);
		}
	}

	public void UpdateActionsList(string text)
	{
		var actionsList = GameObject.FindWithTag("ActionsListUI");

		if (actionsList)
		{
			var textComponent = actionsList.transform.GetChild(1).GetChild(0).GetComponent<Text>();
			if (textComponent != null) textComponent.text += textComponent.text == "" ? text : "\n" + text;

			var scrollRectComponent = actionsList.transform.GetChild(1).GetComponent<ScrollRect>();
			if (scrollRectComponent != null) scrollRectComponent.normalizedPosition = new Vector2(0, 0);
		}
	}

	public void UpdateReply(string text)
	{
		var reply = GameObject.FindWithTag("ReplyUI");

		if (reply)
		{
			var textComponent = reply.GetComponent<Text>();
			if (textComponent != null) textComponent.text = text;
		}
	}

	public void DisableAllButtons()
	{
		var buttons = FindObjectsOfType<Button>();

		foreach (var button in buttons) button.interactable = false;
	}

	private void SetUpKnowledgeOptions(List<Task> tasks)
	{
		var knowledgeContent = GameObject.FindWithTag("KnowledgeContent");

		for (var i = 0; i < knowledgeContent.transform.childCount; i++)
		{
			Destroy(knowledgeContent.transform.transform.GetChild(i).gameObject);
		}

		for (var i = 0; i < tasks.Count; i++)
		{
			var task = tasks[i];
			var newTaskOption = Instantiate(uiOption, knowledgeContent.transform, false);
			newTaskOption.name = task.TaskId;
			newTaskOption.tag = "TaskOptionUI";
			var newTaskOptionButtonComponent = newTaskOption.GetComponent<Button>();
			newTaskOptionButtonComponent.interactable = false;

			var newTaskOptionTextComponent = newTaskOption.transform.GetChild(0).GetComponent<Text>();
			newTaskOptionTextComponent.text = "Task " + task.TaskId;
			newTaskOptionTextComponent.color = Color.white;

			if (task.isCompleted)
			{
				var newColorBlock = newTaskOptionButtonComponent.colors;
				newColorBlock.disabledColor = new Color(73 / 255.0f, 209 / 255.0f, 112 / 255.0f);
				newTaskOptionButtonComponent.colors = newColorBlock;
			}
			else
			{
				var newColorBlock = newTaskOptionButtonComponent.colors;
				newColorBlock.disabledColor = Color.black;
				newTaskOptionButtonComponent.colors = newColorBlock;
			}

			var list = new GameObject("Subtasks-List", typeof(RectTransform));
			list.transform.SetParent(knowledgeContent.transform, false);

			var listVerticalLayoutGroup = list.AddComponent<VerticalLayoutGroup>();
			var listContentSizeFitter = list.AddComponent<ContentSizeFitter>();

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
				newSubtaskOption.tag = "SubtaskOptionUI";
				var newSubtaskOptionButtonComponent = newSubtaskOption.GetComponent<Button>();
				newSubtaskOptionButtonComponent.onClick.AddListener(() => { UIOptionClick(UIOptionType.Subtask, new dynamic[] {task, subtask}); });

				var newSubtaskOptionTextComponent = newSubtaskOption.transform.GetChild(0).GetComponent<Text>();
				newSubtaskOptionTextComponent.text = "Subtask " + task.Subtasks[j].SubtaskId;

				if (subtask.isCompleted)
				{
					var newColorBlock = newSubtaskOptionButtonComponent.colors;
					newColorBlock.normalColor = new Color(73 / 255.0f, 209 / 255.0f, 112 / 255.0f);
					newSubtaskOptionButtonComponent.colors = newColorBlock;

					newSubtaskOptionTextComponent.color = Color.white;
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
			for (var i = 0; i < queries.Count; i++)
			{
				var query = queries[i];
				var newQueryOption = Instantiate(uiOption, queriesContent.transform, false);
				newQueryOption.name = query.Filename;
				newQueryOption.tag = "QueryOptionUI";
				newQueryOption.GetComponent<Button>().onClick.AddListener(() => { UIOptionClick(UIOptionType.Query, new dynamic[] {query}); });

				var newQueryOptionTextComponent = newQueryOption.transform.GetChild(0).GetComponent<Text>();
				newQueryOptionTextComponent.text = query.Filename;
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

				ContextManager.Instance.SetCurrentTask(task);
				ContextManager.Instance.SetCurrentSubtask(subtask);
				ContextManager.Instance.SetCurrentInstruction(instruction);
				break;
			case UIOptionType.Subtask:
				task = data[0] as Task;
				subtask = data[1] as Subtask;
				instruction = subtask?.Instructions.Count > 0 ? subtask.Instructions[0] : null;

				ContextManager.Instance.SetCurrentTask(task);
				ContextManager.Instance.SetCurrentSubtask(subtask);
				ContextManager.Instance.SetCurrentInstruction(instruction);
				break;
			case UIOptionType.Query:
				query = data[0] as Query;
				ContextManager.Instance.SetCurrentQuery(query);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}

		SceneManager.Instance.LoadScene(SceneManager.MainSceneNameGlobal);
	}
}