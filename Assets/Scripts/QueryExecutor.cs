using System;
using System.Collections.Generic;
using System.Linq;
using Catalogs;
using Custom;
using UnityEngine;

public class QueryExecutor : Singleton<QueryExecutor>
{
    public void ExecuteQuery()
    {
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
}