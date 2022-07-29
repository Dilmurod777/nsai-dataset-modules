using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Catalogs;
using Custom;
using UnityEngine;

public class QueryExecutor : Singleton<QueryExecutor>
{
	public bool isQueryExecuting;

	public void ExecuteQuery()
	{
		KnowledgeManager.Instance.isExecuting = true;
		UIManager.Instance.UpdateUI();

		var query = ContextManager.Instance.CurrentQuery;
		var programs = query.Programs;

		if (programs == null || programs.Count == 0)
		{
			Debug.Log("programs empty");
			return;
		}

		foreach (var program in programs)
		{
			var data = program.Split(' ');
			var methodName = data[0];
			var parameters = data.Skip(1).Take(data.Length - 1).ToArray();
			var joinedParameters = string.Join(Constants.ArgsSeparator.ToString(), parameters);

			var methodInfo = Catalog.Instance.GetType().GetMethod(methodName);
			if (methodInfo != null)
			{
				ContextManager.Instance.Prev = methodInfo.Invoke(Catalog.Instance, new object[] {joinedParameters});
			}
		}
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

		StartCoroutine(Sequence(finalList));
	}

	private IEnumerator Sequence(List<IEnumerator> list)
	{
		foreach (var c in list)
		{
			yield return StartCoroutine(c);
		}

		yield return null;
	}
}