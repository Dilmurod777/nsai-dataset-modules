using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Custom
{
    public static class Helpers
    {
        public static string GetFigurePlainName(string figureFullName)
        {
            var subtaskIdsToFigurePrefab = new Dictionary<string, string>
            {
                { "32-11-61", "MainLandingGear" },
                { "32-45-11", "WheelAndTire" }
            };

            var prefabName = "";

            foreach (var key in subtaskIdsToFigurePrefab.Keys)
                if (figureFullName.Contains(key))
                    prefabName = subtaskIdsToFigurePrefab[key];

            var needsToReplace = new List<string> { "IFM", "RFM", "Reference", "Scattered" };

            foreach (var word in needsToReplace) prefabName = prefabName.Replace(word, "");

            return prefabName;
        }

        public static string GetCurrentFigurePlainName()
        {
            var subtask = ContextManager.Instance.CurrentSubtask;
            var subtaskId = subtask.SubtaskId;

            return GetFigurePlainName(subtaskId);
        }

        public static GameObject FindObjectInFigure(Constants.FigureType type, string objName)
        {
            var task = ContextManager.Instance.CurrentTask;
            var taskType = ContextManager.GetTaskType(task);

            var plainFigureName = GetCurrentFigurePlainName();

            var figureName = type switch
            {
                Constants.FigureType.Current => plainFigureName + taskType,
                Constants.FigureType.Ifm => plainFigureName + Constants.FigureType.Ifm,
                Constants.FigureType.Rfm => plainFigureName + Constants.FigureType.Rfm,
                Constants.FigureType.Reference => plainFigureName + Constants.FigureType.Reference,
                _ => plainFigureName + Constants.TaskType.Installation
            };

            var figure = GameObject.Find(figureName);

            foreach (var child in figure.GetComponentsInChildren<Transform>())
            {
                if (child.name.ToLower().Contains(objName.ToLower()))
                    return child.gameObject;
            }

            return null;
        }

        public static void WriteToLogs(Dictionary<string, object> values)
        {
            var fileSuffix = "undefined";
            var filePrefix = "undefined";
            if (ContextManager.Instance.CurrentSubtask != null)
            {
                fileSuffix = ContextManager.Instance.CurrentSubtask.SubtaskId;
                filePrefix = "subtask";
            }

            if (ContextManager.Instance.CurrentQuery != null)
            {
                fileSuffix = ContextManager.Instance.CurrentQuery.Filename + '-' + ContextManager.Instance.CurrentQuery.SubtaskId;
                filePrefix = "query";
            }

            var path = "Assets/Resources/Logs/" + filePrefix + "-" + fileSuffix + ".txt";
            var writer = new StreamWriter(path, true);
            writer.WriteLine("---------------------------------------------");
            foreach (var value in values)
            {
                writer.WriteLine(value.Key + ": " + value.Value);
            }

            writer.Close();
        }
    }
}