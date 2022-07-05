using System;
using System.Collections.Generic;
using Instances;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject uiOption;
    public GameObject knowledgeContent;
    public GameObject queriesContent;

    public enum UIOptionType
    {
        Task,
        Subtask,
        Query
    }

    public void SetUpMenus(List<Task> tasks, List<Query> queries)
    {
        SetUpKnowledgeOptions(tasks);
        SetUpQueryOptions(queries);
    }

    public void UpdateUI()
    {
        if (RootManager.Instance.contextManager.CurrentQuery != null)
        {
            var queryText = GameObject.FindWithTag("QueryText");

            if (queryText != null)
            {
                queryText.GetComponent<InputField>().text = RootManager.Instance.contextManager.CurrentQuery.Title;
            }
        }
    }

    private void SetUpKnowledgeOptions(List<Task> tasks)
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
                newSubtaskOption.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIOptionClick(UIOptionType.Subtask, new dynamic[] {task, subtask});
                });

                var newSubtaskOptionTextComponent = newSubtaskOption.transform.GetChild(0).GetComponent<Text>();
                newSubtaskOptionTextComponent.text = "Subtask " + task.Subtasks[j].SubtaskId;
            }
        }

        var knowledgeScrollRect = knowledgeContent.transform.parent.GetComponent<ScrollRect>();
        knowledgeScrollRect.normalizedPosition = new Vector2(0, 1);
    }

    private void SetUpQueryOptions(List<Query> queries)
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

        var queryScrollRect = queriesContent.transform.parent.GetComponent<ScrollRect>();
        queryScrollRect.normalizedPosition = new Vector2(0, 1);
    }

    private void UIOptionClick(UIOptionType type, dynamic[] data)
    {
        Task task;
        Subtask subtask;
        Query query;

        switch (type)
        {
            case UIOptionType.Task:
                task = data[0] as Task;
                RootManager.Instance.contextManager.SetCurrentTask(task);
                break;
            case UIOptionType.Subtask:
                task = data[0] as Task;
                subtask = data[1] as Subtask;
                RootManager.Instance.contextManager.SetCurrentTask(task);
                RootManager.Instance.contextManager.SetCurrentSubtask(subtask);
                break;
            case UIOptionType.Query:
                query = data[0] as Query;
                RootManager.Instance.contextManager.SetCurrentQuery(query);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        RootManager.Instance.sceneManager.LoadMainScene();
    }
}