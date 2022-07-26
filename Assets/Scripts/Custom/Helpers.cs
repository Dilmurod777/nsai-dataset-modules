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

			var needsToReplace = new List<string> {"-IFM", "-RFM", "-Reference", "-Scattered"};

			foreach (var word in needsToReplace) prefabName = prefabName.Replace(word, "");

			return prefabName;
		}

		public static string GetCurrentFigurePlainName()
		{
			var subtask = ContextManager.Instance.CurrentSubtask;
			var subtaskId = subtask.SubtaskId;

			return GetFigurePlainName(subtaskId);
		}

		public static GameObject FindObjectInFigure(AssetManager.FigureType type, string objName)
		{
			var task = ContextManager.Instance.CurrentTask;
			var taskType = ContextManager.GetTaskType(task);

			var plainFigureName = Helpers.GetCurrentFigurePlainName();

			var figure = type switch
			{
				AssetManager.FigureType.Current => taskType == ContextManager.TaskType.Installation
					? GameObject.Find(plainFigureName + "-Installation")
					: GameObject.Find(plainFigureName + "-Removal"),
				AssetManager.FigureType.IFM => GameObject.Find(plainFigureName + "-IFM"),
				AssetManager.FigureType.RFM => GameObject.Find(plainFigureName + "-RFM"),
				AssetManager.FigureType.Reference => GameObject.Find(plainFigureName + "-Reference"),
				AssetManager.FigureType.Scattered => GameObject.Find(plainFigureName + "-Scattered"),
				_ => GameObject.Find(plainFigureName + "-Installation")
			};

			foreach (var child in figure.GetComponentsInChildren<Transform>())
			{
				if (child.name.Contains(objName))
					return child.gameObject;
			}

			return null;
		}
	}
}