using System.Collections.Generic;
using Constants;
using Custom;

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
			// var argsList = args.Split(General.ArgsSeparator);
			// var attr = argsList[0];
			// var attrValue = ContextManager.Instance.GetAttribute(argsList[1]).ToString();
			// List<JSONNode> dataObjects = ContextManager.Instance.GetAttribute(argsList[2]);
			//
			// var resultObjects = new List<JSONNode>();
			// for (var i = 0; i < dataObjects.Count; i++)
			// {
			// 	if (dataObjects[i][attr] == attrValue)
			// 	{
			// 		resultObjects.Add(dataObjects[i]);
			// ScriptExecutor.SetKnowledgeText(HelperFunctions.GetValueFromJSONNodeByKey("content", dataObjects[i]));
			// ScriptExecutor.SetKnowledgeInfo();


			// if (attr == "order")
			// {
			// 	Context.Instance.CurrentInstructionOrder += 1;
			// }
			// 	}
			// }
			//
			// return resultObjects;
			return new List<JSONNode>();
		}

		public List<JSONNode> FilterType(string args)
		{
			// var argsList = args.Split(General.ArgsSeparator);
			// var type = argsList[0];
			// List<JSONNode> dataObjects = ContextManager.Instance.GetAttribute(argsList[1]);
			//
			// var resultObjects = new List<JSONNode>();
			// foreach (var dataObject in dataObjects)
			// {
			// 	foreach (var item in dataObject[type])
			// 	{
			// 		resultObjects.Add(item);
			// 	}
			// }
			//
			// return resultObjects;
			return new List<JSONNode>();
		}

		public string QueryAttr(string args)
		{
			// var argsList = args.Split(General.ArgsSeparator);
			// var attr = argsList[0];
			// JSONNode dataObject = ContextManager.Instance.GetAttribute(argsList[1]);
			//
			// string result = dataObject[attr];
			// return result;

			return "";
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