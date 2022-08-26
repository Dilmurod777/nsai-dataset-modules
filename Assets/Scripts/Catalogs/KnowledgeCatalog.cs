using System.Collections;
using System.Collections.Generic;
using Custom;
using UnityEngine;
using static Constants;

namespace Catalogs
{
    public interface IKnowledgeCatalogInterface
    {
        JSONNode FilterAttr(string args);
        List<JSONNode> FilterType(string args);
        string QueryAttr(string args);
        void ShowInfo(string args);
    }

    public class KnowledgeCatalog : IKnowledgeCatalogInterface
    {
        public JSONNode FilterAttr(string args)
        {
            Debug.Log("FilterAttr: " + args);
            var argsList = args.Split(ArgsSeparator);
            var attr = argsList[0];
            var attrValue = ContextManager.Instance.GetAttribute<string>(argsList[1]);
            var dataObjects = ContextManager.Instance.GetAttribute<List<JSONNode>>(argsList[2]);

            var resultObjects = new JSONArray();
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
            Debug.Log("FilterType: " + args);
            var argsList = args.Split(ArgsSeparator);
            var type = argsList[0];
            var dataObjects = ContextManager.Instance.GetAttribute<JSONNode>(argsList[1]);

            var resultObjects = new List<JSONNode>();
            if (dataObjects == null) return resultObjects;

            foreach (var dataObject in dataObjects)
            {
                foreach (var item in dataObject.Value[type])
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
            var dataObject = ContextManager.Instance.GetAttribute<JSONNode>(argsList[1]);

            if (dataObject == null) return "";

            string result = dataObject[attr];

            if (attr == "figure" && string.IsNullOrEmpty(result))
            {
                return ContextManager.Instance.CurrentTask.TaskId;
            }

            return result;
        }

        public void ShowInfo(string args)
        {
            Debug.Log("Show Info: " + args);
            var argsList = args.Split(ArgsSeparator);
            var dataObjects = ContextManager.Instance.GetAttribute<System.Object>(argsList[0]);

            if (dataObjects is JSONArray)
            {
                var result = "";
                foreach (var dataObject in (JSONArray)dataObjects)
                {
                    if (!dataObject.Value.IsArray)
                    {
                        result +=dataObject.Value["content"] + "\n";
                    }
                }

                UIManager.UpdateReply(result);
            }
            else if (dataObjects is string)
            {
                UIManager.UpdateReply((string)dataObjects);
            }
            else
            {
                Debug.Log("Show Info error: " + dataObjects.GetType());
                UIManager.UpdateReply("");
            }
        }
    }
}