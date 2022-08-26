using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Catalogs;
using Custom;
using Instances;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Exception = System.Exception;

public class QueryExecutor : Singleton<QueryExecutor>
{
    public string currentQueryText = "";
    public static bool IsTextInputUpdated = false;

    public IEnumerator ExecuteQuery()
    {
        KnowledgeManager.Instance.isExecuting = true;
        UIManager.Instance.UpdateUI();

        var query = ContextManager.Instance.CurrentQuery;
        var programs = new List<string>();

        var queryInputField = GameObject.FindWithTag(Tags.QueryText);
        if (queryInputField == null)
        {
            KnowledgeManager.Instance.isExecuting = false;
            UIManager.Instance.UpdateUI();
            yield break;
        }

        if (IsTextInputUpdated)
        {
            if (currentQueryText == "")
            {
                KnowledgeManager.Instance.isExecuting = false;
                UIManager.Instance.UpdateUI();
                yield break;
            }

            var form = new WWWForm();
            form.AddField("query", currentQueryText);

            Debug.Log("Text: " + currentQueryText);
            var www = UnityWebRequest.Post("http://165.246.43.139:8000/predict", form);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                var data = JSON.Parse(www.downloadHandler.text);

                Debug.Log(data);
                foreach (var item in data["programs"])
                {
                    programs.Add(item.Value);
                }

                yield return ExecutePrograms(programs);
                KnowledgeManager.Instance.isExecuting = false;
                UIManager.Instance.UpdateUI();
                yield break;
            }
            else
            {
                Debug.Log("Server error: " + www.error);
                KnowledgeManager.Instance.isExecuting = false;
                UIManager.Instance.UpdateUI();
                yield break;
            }
        }

        if (query == null) yield break;

        programs = query.Programs;
        yield return ExecutePrograms(programs);

        KnowledgeManager.Instance.isExecuting = false;
        UIManager.Instance.UpdateUI();
        yield return null;
    }

    public IEnumerator ExecutePrograms(List<string> programs)
    {
        if (programs == null || programs.Count == 0)
        {
            Debug.Log("programs empty");
            StopCoroutine(ExecuteQuery());
            yield break;
        }

        var query = ContextManager.Instance.CurrentQuery;
        query.Programs = programs;
        ContextManager.Instance.SetCurrentQuery(query);

        foreach (var program in programs)
        {
            var data = program.Split(' ');
            var methodName = data[0];
            var parameters = data.Skip(1).Take(data.Length - 1).ToArray();
            var joinedParameters = string.Join(Constants.ArgsSeparator.ToString(), parameters);

            var methodInfo = Catalog.Instance.GetType().GetMethod(methodName);
            if (methodInfo != null)
            {
                yield return ContextManager.Instance.Prev = methodInfo.Invoke(Catalog.Instance, new object[] { joinedParameters });
            }
        }
        
        UIManager.Instance.UpdateUI();
    }

    public void UpdateQueryText(string value)
    {
        IsTextInputUpdated = true;
        var query = ContextManager.Instance.CurrentQuery;
        var task = ContextManager.Instance.CurrentTask;
        var subtask = ContextManager.Instance.CurrentSubtask;
        var instruction = ContextManager.Instance.CurrentInstruction;
        if (query == null)
        {
            query = new Query("", value, new List<string>(), "", task.TaskId, subtask.SubtaskId, instruction.Order);
        }

        query.Title = value;
        ContextManager.Instance.SetCurrentQuery(query);

        currentQueryText = value;
        UIManager.Instance.UpdateQueryPlayButtonState();
    }

    public void RunCoroutine(IEnumerator callback)
    {
        RunCoroutinesInSequence(new List<IEnumerator>
        {
            callback
        });
    }

    public void RunCoroutinesInSequence(List<IEnumerator> callbacks)
    {
        var finalList = new List<IEnumerator>();
        finalList.AddRange(callbacks);
        finalList.Add(PrimitiveManager.SimplePrimitive(() =>
        {
            KnowledgeManager.Instance.isExecuting = false;
            UIManager.Instance.UpdateUI();
        }));

        StartCoroutine(PrimitiveManager.Instance.Sequence(finalList));
    }
}