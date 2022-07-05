using System.Collections.Generic;
using Instances;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject uiOption;
    public GameObject knowledgeContent;
    public GameObject queriesContent;

    public void SetUpMenus(List<Task> tasks, List<Query> queries)
    {
        SetUpKnowledgeOptions(tasks);
        SetUpQueryOptions(queries);
    }

    private void SetUpKnowledgeOptions(List<Task> tasks)
    {
        for (var i = 0; i < tasks.Count; i++)
        {
            var newTaskOption = Instantiate(uiOption, knowledgeContent.transform, false);
            newTaskOption.name = tasks[i].TaskId;

            var newTaskOptionTextComponent = newTaskOption.transform.GetChild(0).GetComponent<Text>();
            newTaskOptionTextComponent.text = "Task " + tasks[i].TaskId;

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

            for (var j = 0; j < tasks[i].Subtasks.Count; j++)
            {
                var newSubtaskOption = Instantiate(uiOption, list.transform, false);
                newSubtaskOption.name = tasks[i].Subtasks[j].SubtaskId;

                var newSubtaskOptionTextComponent = newSubtaskOption.transform.GetChild(0).GetComponent<Text>();
                newSubtaskOptionTextComponent.text = "Subtask " + tasks[i].Subtasks[j].SubtaskId;
            }
        }

        var knowledgeScrollRect = knowledgeContent.transform.parent.GetComponent<ScrollRect>();
        knowledgeScrollRect.normalizedPosition = new Vector2(0, 1);
    }
    
    private void SetUpQueryOptions(List<Query> queries)
    {
        for (var i = 0; i < queries.Count; i++)
        {
            var newQueryOption = Instantiate(uiOption, queriesContent.transform, false);
            newQueryOption.name = queries[i].Filename;

            var newQueryOptionTextComponent = newQueryOption.transform.GetChild(0).GetComponent<Text>();
            newQueryOptionTextComponent.text = queries[i].Filename;
        }

        var queryScrollRect = queriesContent.transform.parent.GetComponent<ScrollRect>();
        queryScrollRect.normalizedPosition = new Vector2(0, 1);
    }
}