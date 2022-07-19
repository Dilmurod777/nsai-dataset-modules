using Custom;
using UnityEngine;

public class QueryExecutor : Singleton<QueryExecutor>
{
	public void ExecuteQuery()
	{
		var query = ContextManager.Instance.CurrentQuery;
		Debug.Log(query.Title);
	}
}