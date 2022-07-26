using System.Collections.Generic;
using Custom;
using UnityEngine;
using static Constants;

namespace Catalogs
{
	public interface IKnowledgeCatalogInterface
	{
		List<JSONNode> FilterAttr(string args);
		List<JSONNode> FilterType(string args);
		string QueryAttr(string args);
		string ShowInfo(List<JSONNode> dataObjects);
	}

	public class KnowledgeCatalog : IKnowledgeCatalogInterface
	{
		public List<JSONNode> FilterAttr(string args)
		{
			var argsList = args.Split(ArgsSeparator);
			var attr = argsList[0];
			var attrValue = ContextManager.Instance.GetAttribute(argsList[1]).ToString();
			var dataObjects = ContextManager.Instance.GetAttribute(argsList[2]) as List<JSONNode>;
			
			var resultObjects = new List<JSONNode>();
			if (dataObjects == null) return resultObjects;
			
			for (var i = 0; i < dataObjects.Count; i++)
			{
				if (dataObjects[i][attr] == attrValue)
				{
					resultObjects.Add(dataObjects[i]);
				}
			}

			return resultObjects;
		}

		public List<JSONNode> FilterType(string args)
		{
			var argsList = args.Split(ArgsSeparator);
			var type = argsList[0];
			var dataObjects = ContextManager.Instance.GetAttribute(argsList[1]) as List<JSONNode>;
			
			var resultObjects = new List<JSONNode>();
			if (dataObjects == null) return resultObjects;
			
			foreach (var dataObject in dataObjects)
			{
				foreach (var item in dataObject[type])
				{
					resultObjects.Add(item);
				}
			}

			return resultObjects;
		}

		public string QueryAttr(string args)
		{
			var argsList = args.Split(ArgsSeparator);
			var attr = argsList[0];
			var dataObject = ContextManager.Instance.GetAttribute(argsList[1]) as JSONNode;

			if (dataObject == null) return "";
			
			string result = dataObject[attr];
			return result;
		}

		public string ShowInfo(List<JSONNode> dataObjects)
		{
			var result = "";
			foreach (var dataObject in dataObjects)
			{
				foreach (var item in dataObject)
				{
					if (!item.Value.IsArray)
					{
						result += item.Key + ": " + item.Value + "\n";
					}
				}
			}

			return result;
		}
	}
}