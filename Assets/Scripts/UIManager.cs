using System;
using System.Collections.Generic;
using Custom;
using Instances;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	public enum UIOptionType
	{
		Task,
		Subtask,
		Query
	}

	public GameObject uiOption;
	public GameObject uiAction;
	public GameObject uiBasicOperation;

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
			homeButton.GetComponent<Button>().onClick.AddListener(() => { SceneManager.Instance.LoadScene(SceneManager.MenuSceneNameGlobal); });
			homeButton.GetComponent<Button>().interactable = !KnowledgeManager.Instance.isExecutingSubtask;
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
			nextButton.GetComponent<Button>().interactable = KnowledgeManager.HasNextSubtask() && !KnowledgeManager.Instance.isExecutingSubtask;
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
			previousButton.GetComponent<Button>().interactable = KnowledgeManager.HasPreviousSubtask() && !KnowledgeManager.Instance.isExecutingSubtask;
		}

		var resetButton = GameObject.FindWithTag("ResetButton");
		if (resetButton != null)
		{
			resetButton.GetComponent<Button>().onClick.RemoveAllListeners();
			resetButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				AssetManager.Instance.ResetFigures();
				KnowledgeManager.Instance.ResetTasks();
				SceneManager.Instance.LoadScene(SceneManager.MenuSceneNameGlobal);
			});
			resetButton.GetComponent<Button>().interactable = !KnowledgeManager.Instance.isExecutingSubtask;
		}

		var queryPlayButton = GameObject.FindWithTag("QueryPlayButton");
		if (queryPlayButton != null)
		{
			queryPlayButton.GetComponent<Button>().onClick.RemoveAllListeners();
			queryPlayButton.GetComponent<Button>().onClick.AddListener(() => { QueryExecutor.Instance.ExecuteQuery(); });
			queryPlayButton.GetComponent<Button>().interactable = !KnowledgeManager.Instance.isExecutingSubtask && ContextManager.Instance.CurrentQuery != null;
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
		if (knowledgeTaskUI)
		{
			knowledgeTaskUI.transform.GetChild(0).GetComponent<Image>().enabled = ContextManager.Instance.CurrentTask != null;
			knowledgeTaskUI!.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentTask != null
				? "Task " + ContextManager.Instance.CurrentTask.TaskId
				: "";

			knowledgeTaskUI.transform.GetChild(1).GetComponent<Image>().enabled = ContextManager.Instance.CurrentTask != null;
			knowledgeTaskUI!.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentTask != null
				? ContextManager.Instance.CurrentTask.Title
				: "";
		}


		var knowledgeSubtaskUI = GameObject.FindWithTag("KnowledgeSubtaskUI");
		if (knowledgeSubtaskUI)
		{
			knowledgeSubtaskUI.transform.GetChild(0).GetComponent<Image>().enabled = ContextManager.Instance.CurrentSubtask != null;
			knowledgeSubtaskUI!.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentSubtask != null
				? "Subtask " + ContextManager.Instance.CurrentSubtask.SubtaskId
				: "";

			knowledgeSubtaskUI.transform.GetChild(1).GetComponent<Image>().enabled = ContextManager.Instance.CurrentSubtask != null;
			knowledgeSubtaskUI!.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentSubtask != null
				? ContextManager.Instance.CurrentSubtask.Content
				: "";
			knowledgeSubtaskUI.transform.GetChild(1).GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
		}

		var knowledgeInstructionUI = GameObject.FindWithTag("KnowledgeInstructionUI");
		if (knowledgeInstructionUI != null)
		{
			knowledgeInstructionUI!.GetComponent<Image>().enabled = ContextManager.Instance.CurrentInstruction != null;
			knowledgeInstructionUI!.transform.GetChild(0).GetComponent<Text>().text = ContextManager.Instance.CurrentInstruction != null
				? ContextManager.Instance.CurrentInstruction.Content
				: "";
			knowledgeInstructionUI.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
		}

		var knowledgeActionsUI = GameObject.FindWithTag("KnowledgeActionsUI");
		if (knowledgeActionsUI != null)
		{
			knowledgeActionsUI!.transform.GetChild(0).GetChild(0).GetComponent<Text>().enabled =
				ContextManager.Instance.CurrentInstruction != null && ContextManager.Instance.CurrentInstruction.Actions.Count > 0;
			knowledgeActionsUI!.transform.GetChild(0).GetComponent<Image>().enabled = ContextManager.Instance.CurrentInstruction != null &&
			                                                                          ContextManager.Instance.CurrentInstruction.Actions.Count > 0;
			var actions = ContextManager.Instance.CurrentInstruction != null
				? ContextManager.Instance.CurrentInstruction.Actions
				: null;

			var contentScrollList = knowledgeActionsUI.transform.GetChild(1).GetChild(0);
			if (actions != null)
			{
				for (var i = 0; i < contentScrollList.childCount; i++) Destroy(contentScrollList.GetChild(i).gameObject);

				foreach (var action in actions)
				{
					var obj = Helpers.FindObjectInFigure(Constants.FigureType.Current, action.Components[0]);

					var finalObj = obj.transform.childCount > 1 ? obj.transform.GetChild(0).gameObject : obj;

					var objectMeta = finalObj.GetComponent<ObjectMeta>();

					var newUIAction = Instantiate(uiAction, contentScrollList.transform, false);

					newUIAction.transform.GetChild(0).GetComponent<Text>().text =
						objectMeta.attachType + " " + action.Components[0] + "," + action.Components[1];
				}
			}
			else
			{
				for (var i = 0; i < contentScrollList.childCount; i++) Destroy(contentScrollList.transform.GetChild(i).gameObject);
			}
		}
	}

	public void UpdateBasicOperationsList(string text)
	{
		var basicOperationsList = GameObject.FindWithTag("BasicOperationsUI");

		if (basicOperationsList)
		{
			basicOperationsList.transform.GetChild(0).GetComponent<Text>().enabled = true;

			var content = basicOperationsList.transform.GetChild(1).GetChild(0);

			var newBasicOperationUI = Instantiate(uiBasicOperation, content.transform, false);
			newBasicOperationUI.transform.GetChild(0).GetComponent<Text>().text = text;

			Invoke(nameof(ScrollBasicOperationsListToBottom), 0.25f);
		}
	}

	private void ScrollBasicOperationsListToBottom()
	{
		var basicOperationsList = GameObject.FindWithTag("BasicOperationsUI");
		basicOperationsList.transform.GetChild(1).GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
	}

	public static void ResetBasicOperationsList()
	{
		var basicOperationsList = GameObject.FindWithTag("BasicOperationsUI");
		if (!basicOperationsList) return;

		basicOperationsList.transform.GetChild(0).GetComponent<Text>().enabled = false;

		var content = basicOperationsList.transform.GetChild(1).GetChild(0);

		for (var i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);
	}

	public static void UpdateReply(string text)
	{
		var reply = GameObject.FindWithTag("ReplyUI");
		if (!reply) return;

		var textComponent = reply.GetComponent<Text>();
		if (textComponent != null) textComponent.text = text;
	}

	public static void DisableAllButtons()
	{
		var buttons = FindObjectsOfType<Button>();

		foreach (var button in buttons) button.interactable = false;
	}

	private void SetUpKnowledgeOptions(List<Task> tasks)
	{
		var knowledgeContent = GameObject.FindWithTag("KnowledgeContent");

		if (knowledgeContent.transform.childCount == 0)
		{
			foreach (var task in tasks)
			{
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

				foreach (var subtask in task.Subtasks)
				{
					var newSubtaskOption = Instantiate(uiOption, list.transform, false);
					newSubtaskOption.name = subtask.SubtaskId;
					newSubtaskOption.tag = "SubtaskOptionUI";
					var newSubtaskOptionButtonComponent = newSubtaskOption.GetComponent<Button>();
					var task1 = task;
					newSubtaskOptionButtonComponent.onClick.AddListener(() => { UIOptionClick(UIOptionType.Subtask, new dynamic[] {task1, subtask}); });

					var newSubtaskOptionTextComponent = newSubtaskOption.transform.GetChild(0).GetComponent<Text>();
					newSubtaskOptionTextComponent.text = "Subtask " + subtask.SubtaskId;

					if (!subtask.isCompleted) continue;

					var newColorBlock = newSubtaskOptionButtonComponent.colors;
					newColorBlock.normalColor = new Color(73 / 255.0f, 209 / 255.0f, 112 / 255.0f);
					newSubtaskOptionButtonComponent.colors = newColorBlock;

					newSubtaskOptionTextComponent.color = Color.white;

					Invoke(nameof(ScrollKnowledgeToTop), 0.001f);
				}
			}

			Invoke(nameof(ScrollKnowledgeToTop), 0.001f);
		}
		else
		{
			Invoke(nameof(ScrollKnowledgeToTop), 0.001f);
		}
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
				newQueryOption.tag = "QueryOptionUI";
				newQueryOption.GetComponent<Button>().onClick.AddListener(() => { UIOptionClick(UIOptionType.Query, new dynamic[] {query}); });

				var newQueryOptionTextComponent = newQueryOption.transform.GetChild(0).GetComponent<Text>();
				newQueryOptionTextComponent.text = query.Filename;

				Invoke(nameof(ScrollQueriesToTop), 0.001f);
			}

			Invoke(nameof(ScrollQueriesToTop), 0.001f);
		}
		else
		{
			Invoke(nameof(ScrollQueriesToTop), 0.001f);
		}
	}

	private void ScrollKnowledgeToTop()
	{
		var knowledgeContent = GameObject.FindWithTag("KnowledgeContent");
		var knowledgeScrollRect = knowledgeContent.transform.parent.GetComponent<ScrollRect>();

		knowledgeScrollRect!.normalizedPosition = new Vector2(0, 1);
	}

	private void ScrollQueriesToTop()
	{
		var queriesContent = GameObject.FindWithTag("QueriesContent");
		var queryScrollRect = queriesContent.transform.parent.GetComponent<ScrollRect>();

		queryScrollRect.normalizedPosition = new Vector2(0, 1);
	}


	private void UIOptionClick(UIOptionType type, dynamic[] data)
	{
		Task task = null;
		Subtask subtask = null;
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

		if (task != null && subtask != null)
		{
			var figurePlainName = Helpers.GetCurrentFigurePlainName();
			var taskType = ContextManager.GetTaskType(task);

			var figure = GameObject.Find(figurePlainName + taskType);
			AssetManager.Instance.ResetFigure(figure);
		}

		SceneManager.Instance.LoadScene(SceneManager.MainSceneNameGlobal);
	}
}