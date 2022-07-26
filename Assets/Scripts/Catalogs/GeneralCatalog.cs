﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Instances;
using UnityEngine;

namespace Catalogs
{
    public interface IGeneralCatalogInterface
    {
        object SaveVal2Var(string args);
        int Count(object[] objects);
        bool Exist(object[] objects);
        object Unique(string args);
        List<string> ExtractNumbers(string args);
        List<string> ExtractID(string args);
        string Same(string args);
    }

    public class GeneralCatalog : IGeneralCatalogInterface
    {
        public object SaveVal2Var(string args)
        {
            Debug.Log("SaveVal2Var: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var source = ContextManager.Instance.GetAttribute(argsList[0]);
            var varName = argsList[1];

            switch (varName)
            {
                case "var1":
                    ContextManager.Instance.Var1 = source;
                    break;
                case "var2":
                    ContextManager.Instance.Var2 = source;
                    break;
            }

            return source;
        }

        public int Count(object[] objects)
        {
            Debug.Log("Count");
            return objects.Length;
        }

        public bool Exist(object[] objects)
        {
            Debug.Log("Exist");
            return objects.Length > 0;
        }

        public object Unique(string args)
        {
            Debug.Log("Unique: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var objs = ContextManager.Instance.GetAttribute(argsList[0]) as List<object>;

            if (objs == null || objs.Count == 0) return null;

            return objs[0];
        }

        public List<string> ExtractNumbers(string args)
        {
            Debug.Log("ExtractNumbers: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var query = ContextManager.Instance.GetAttribute(argsList[0]) as Query;

            if (query == null) return null;

            var title = query.Title;
            var figureIds = Regex.Matches(title, Constants.FigureRegex);
            for (var i = 0; i < figureIds.Count; i++)
            {
                title = title.Replace(figureIds[i].Value, "");
            }

            var objectIds = Regex.Matches(title, Constants.ObjectRegex);
            for (var i = 0; i < objectIds.Count; i++)
            {
                title = title.Replace(objectIds[i].Value, "");
            }

            var numbers = Regex.Matches(title, Constants.NumberRegex);
            var result = new List<string>();
            for (var i = 0; i < numbers.Count; i++)
            {
                if (numbers[i].Value == "") continue;
                result.Add(numbers[i].Value);
            }

            return result;
        }

        public List<string> ExtractID(string args)
        {
            Debug.Log("ExtractID: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var attrId = argsList[0];
            var query = ContextManager.Instance.GetAttribute(argsList[1]) as Query;

            var result = new List<string>();
            if (query != null)
            {
                var title = query.Title;
                
                switch (attrId)
                {
                    case "subtask_id":
                    case "task_id":
                        foreach (Match match in Regex.Matches(title, Constants.NumberRegex))
                        {
                            result.Add(match.Value);
                        }

                        break;
                    case "figure":
                        foreach (Match match in Regex.Matches(title, Constants.FigureRegex))
                        {
                            result.Add(match.Value);
                        }

                        break;
                    case "object":
                        foreach (Match match in Regex.Matches(title, Constants.ObjectRegex))
                        {
                            result.Add(match.Value);
                        }

                        break;
                    default:
                        foreach (Match match in Regex.Matches(title, Constants.NumberRegex))
                        {
                            result.Add(match.Value);
                        }

                        break;
                }
            }

            return result;
        }

        public string Same(string args)
        {
            var argsList = args.Split(Constants.ArgsSeparator);
            var var1 = ContextManager.Instance.GetAttribute(argsList[0]) as string;
            var var2 = ContextManager.Instance.GetAttribute(argsList[1]) as string;

            return var1 == "402-32-11-61-990-802-A" ? var1 : "402-32-11-61-990-802-A";
        }
    }
}