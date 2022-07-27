using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
	public static class Helpers
	{
		public static string GetFigurePlainName(string figureFullName)
		{
			var subtaskIdsToFigurePrefab = new Dictionary<string, string>
			{
				{"32-11-61", "MainLandingGear"},
				{"32-45-11", "WheelAndTire"}
			};

			var prefabName = "";

			foreach (var key in subtaskIdsToFigurePrefab.Keys)
				if (figureFullName.Contains(key))
					prefabName = subtaskIdsToFigurePrefab[key];

			var needsToReplace = new List<string> {"IFM", "RFM", "Reference", "Scattered"};

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
				Constants.FigureType.Scattered => plainFigureName + Constants.FigureType.Scattered,
				_ => plainFigureName + Constants.TaskType.Installation
			};

			var figure = GameObject.Find(figureName);

			foreach (var child in figure.GetComponentsInChildren<Transform>())
			{
				if (child.name.Contains(objName))
					return child.gameObject;
			}

			return null;
		}
	}
}