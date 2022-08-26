using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Custom;
using Instances;
using UnityEngine;
using Action = Instances.Action;

namespace Catalogs
{
    public interface IActionsCatalog3DInterface
    {
        List<GameObject> Filter3DAttr(string args);
        void Reset(string args);
        void Highlight(string args);
        void Rotate(string args);
        void Scale(string args);
        void ShowSide(string args);
        void SideBySideLook(string args);
        void CloseLook(string args);
        void Animate(string args);
        void Visibility(string args);
        void Attach(string args);
        void Detach(string args);
        List<Action> CreateActions(string args);
        string CheckActionsValidity(string args);
    }

    public class ActionsCatalog3D : IActionsCatalog3DInterface
    {
        public List<GameObject> Filter3DAttr(string args)
        {
            Debug.Log("Filter3DAttr: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var attrName = argsList[0];
            var prev = ContextManager.Instance.HasAttribute(argsList[1])
                ? ContextManager.Instance.GetAttribute<System.Object>(argsList[1])
                : argsList[1];
            GameObject parent = null;
            if (argsList[2] != "root3D")
            {
                parent = ContextManager.Instance.GetAttribute<GameObject>(argsList[2]);
            }

            var allObjects = new List<GameObject>();
            switch (attrName)
            {
                case "name":
                    var ids = new List<string>();

                    if (prev is string) ids.Add((string)prev);
                    if (prev is IList) ids.AddRange((IList<string>)prev);
                    if (prev is null) ids.Add(ContextManager.Instance.CurrentSubtask?.SubtaskId);

                    // var objects = FindObjectsWithIds(ids, parent);
                    var figures = FindFigureWithId(ids, parent);

                    // allObjects.AddRange(objects);
                    allObjects.AddRange(figures);
                    break;
                case "items":
                    if (prev is IList) allObjects.AddRange(FindObjectsWithIds((IList<string>)prev, parent));
                    if (prev is string)
                        allObjects.AddRange(FindObjectsWithIds(new List<string> { prev.ToString() }, parent));

                    break;
                case "type":
                    if ((string)prev == "figure" && parent != null)
                    {
                        var currentTask = ContextManager.Instance.CurrentTask;
                        var taskType = ContextManager.GetTaskType(currentTask);
                        var plainFigureName = Helpers.GetCurrentFigurePlainName();
                        var figureName = taskType == Constants.TaskType.Installation
                            ? plainFigureName + Constants.TaskType.Installation
                            : plainFigureName + Constants.TaskType.Removal;

                        allObjects.AddRange(FindFigureWithId(new List<string> { figureName }, parent));
                    }

                    break;
            }

            return allObjects;
        }

        private static List<GameObject> FindObjectsWithIds(IList<string> ids, GameObject parent)
        {
            List<GameObject> allObjects;
            if (parent == null)
            {
                allObjects = GameObject.FindGameObjectsWithTag(Tags.Object).ToList();
            }
            else
            {
                allObjects = parent.GetComponentsInChildren<Transform>()
                    .Where(obj => obj.CompareTag(Tags.Object))
                    .Select(obj => obj.gameObject).ToList();
            }

            var currentTask = ContextManager.Instance.CurrentTask;
            var taskType = ContextManager.GetTaskType(currentTask);
            var plainFigureName = Helpers.GetCurrentFigurePlainName();
            var figureName = taskType == Constants.TaskType.Installation
                ? plainFigureName + Constants.TaskType.Installation
                : plainFigureName + Constants.TaskType.Removal;

            var foundObs = new List<GameObject>();

            foreach (var id in ids)
                foundObs.AddRange(allObjects.Where(obj =>
                    (obj.name.Contains("[" + id + "]") || obj.name.Equals(id)) &&
                    obj.transform.parent.name.Equals(figureName)));

            return foundObs;
        }

        private static List<GameObject> FindFigureWithId(List<string> ids, GameObject parent)
        {
            var allFigures = new List<GameObject>();
            if (parent == null)
            {
                allFigures.AddRange(GameObject.FindGameObjectsWithTag(Tags.Figure).ToList());
            }
            else
            {
                allFigures.AddRange(parent.GetComponentsInChildren<Transform>()
                    .Where(obj => obj.CompareTag(Tags.Figure))
                    .Select(obj => obj.gameObject).ToList());
            }

            var foundFigs = new List<GameObject>();

            var currentTask = ContextManager.Instance.CurrentTask;
            var taskType = ContextManager.GetTaskType(currentTask);

            foreach (var fig in allFigures)
                foundFigs.AddRange(from id in ids
                    select Helpers.GetFigurePlainName(id)
                    into plainFigureName
                    select taskType == Constants.TaskType.Installation
                        ? plainFigureName + Constants.TaskType.Installation
                        : plainFigureName + Constants.TaskType.Removal
                    into figureName
                    where fig.name == figureName
                    select fig);

            return foundFigs;
        }

        public void Rotate(string args)
        {
            Debug.Log("Rotate: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var obj = ContextManager.Instance.GetAttribute<GameObject>(argsList[0]);
            if (obj == null) return;

            var restArgs = ContextManager.Instance.GetAttribute<List<string>>(argsList[1]);
            if (restArgs == null) return;

            var degree = float.Parse(restArgs[0]);
            var axisRegex = Regex.Match(ContextManager.Instance.CurrentQuery.Title, @"[XYZ] axis").Value;

            var rotation = obj.transform.rotation;
            var axis = axisRegex.Split(' ')[0];
            var rotationX = axis == "X" ? degree : 0;
            var rotationY = axis == "Y" ? degree : 0;
            var rotationZ = axis == "Z" ? degree : 0;

            var newRotation = rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
            
            UIManager.Instance.UpdateReplyText("Rotating " + obj.name + " by " + degree + " in " + axis + " axis");
            QueryExecutor.Instance.RunCoroutine(Robot.Instance.Rotate(obj, newRotation.eulerAngles));
        }

        public void Scale(string args)
        {
            Debug.Log("Scale: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var obj = ContextManager.Instance.GetAttribute<GameObject>(argsList[1]);
            if (obj == null) return;

            var restArgs = ContextManager.Instance.GetAttribute<List<string>>(argsList[2]);
            var state = argsList[0];
            var scaleRatio = float.Parse(restArgs[0]);

            var currentLocalScale = obj.transform.localScale;
            var currentLocalScaleX = currentLocalScale.x;
            var currentLocalScaleY = currentLocalScale.y;
            var currentLocalScaleZ = currentLocalScale.z;
            var change = scaleRatio < 1
                ? state == "up" ? 1 + scaleRatio : 1 - scaleRatio
                : state == "up"
                    ? scaleRatio
                    : Mathf.Round(100f / scaleRatio) / 100f;

            var finalScale = new Vector3(currentLocalScaleX * change, currentLocalScaleY * change,
                currentLocalScaleZ * change);

            UIManager.Instance.UpdateReplyText("Scaling " + state + " " + obj.name + " by " + scaleRatio);
            QueryExecutor.Instance.RunCoroutine(Robot.Instance.Scale(obj, finalScale));
        }

        public void Reset(string args)
        {
            Debug.Log("Reset: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);

            var obj = ContextManager.Instance.GetAttribute<GameObject>(argsList[0]);
            if (obj == null) return;

            AssetManager.Instance.ResetFigure(obj);
            UIManager.Instance.UpdateReplyText("Resetting " + obj.name);
            QueryExecutor.Instance.RunCoroutine(PrimitiveManager.DelayPrimitive(1.0f));
        }

        public void Highlight(string args)
        {
            Debug.Log("Highlight: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var state = argsList[0];

            var objs = ContextManager.Instance.GetAttribute<List<GameObject>>(argsList[1]);
            if (objs == null || objs.Count == 0) return;
            
            var objNames = new List<string>();
            foreach (var obj in objs)
            {
                objNames.Add(obj.name);
                var outlineComponent = obj.GetComponent<Outline>();
                if (outlineComponent == null) outlineComponent = obj.AddComponent<Outline>();

                switch (state)
                {
                    case "on":
                        outlineComponent.OutlineMode = Outline.Mode.OutlineAll;
                        outlineComponent.OutlineWidth = 5.0f;
                        outlineComponent.OutlineColor = Color.blue;

                        outlineComponent.enabled = true;
                        break;
                    case "off":
                        outlineComponent.enabled = false;
                        break;
                }
            }
            
            switch (state)
            {
                case "on":
                    UIManager.Instance.UpdateReplyText("Highlighting " + string.Join(",", objNames));
                    break;
                case "off":
                    UIManager.Instance.UpdateReplyText("Hiding highlight from " + string.Join(",", objNames));
                    break;
            }
            QueryExecutor.Instance.RunCoroutine(PrimitiveManager.DelayPrimitive(1.0f));
        }

        public void ShowSide(string args)
        {
            Debug.Log("Show side: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var obj = ContextManager.Instance.GetAttribute<GameObject>(argsList[1]);
            if (obj == null) return;

            var figureSide = argsList[0];

            var forward = obj.transform.forward;
            var right = obj.transform.right;
            var up = obj.transform.up;
            var sideRotation = figureSide switch
            {
                "front" => new Vector3(0, 0, 0),
                "back" => new Vector3(0, 0, -180),
                "right" => new Vector3(90, 0, 0),
                "left" => new Vector3(-90, 0, 0),
                "top" => new Vector3(0, 90, 0),
                "bottom" => new Vector3(0, -90, 0),
                _ => new Vector3(0, 0, 0)
            };
            // var angle = Quaternion.Angle(Quaternion.Euler(sideRotation), Quaternion.Euler(Camera.main.transform.forward));
            // var finalRotation = obj.transform.rotation * angle;
            // obj.transform.LookAt(Camera.main.transform.position, sideRotation);

            var camera = Camera.main;
            if (camera != null)
            {
                var cameraRotation = camera.transform.rotation;
                var finalRotation =
                    Quaternion.LookRotation(
                        obj.transform.position + cameraRotation * Vector3.forward + sideRotation);

                QueryExecutor.Instance.RunCoroutine(Robot.Instance.Rotate(obj, finalRotation.eulerAngles));
            }
        }

        public void SideBySideLook(string args)
        {
            Debug.Log("SideBySideLook: " + args);
            var argsList = args.Split(' ');
            var fig = ContextManager.Instance.GetAttribute<GameObject>(argsList[0]);
            if (fig == null) return;

            var objs = new List<GameObject>();
            for (var i = 0; i < fig.transform.childCount; i++)
            {
                objs.Add(fig.transform.GetChild(i).gameObject);
            }

            CloseLookFunctionality(objs);

            QueryExecutor.Instance.RunCoroutine(PrimitiveManager.DelayPrimitive(1.0f));
        }

        public void CloseLook(string args)
        {
            Debug.Log("CloseLook: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var objs = ContextManager.Instance.GetAttribute<List<GameObject>>(argsList[0]);
            if (objs == null || objs.Count == 0) return;

            CloseLookFunctionality(objs);

            QueryExecutor.Instance.RunCoroutine(PrimitiveManager.DelayPrimitive(1.0f));
        }

        public void Animate(string args)
        {
            Debug.Log("Animate: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var fig = ContextManager.Instance.GetAttribute<GameObject>(argsList[1]);
            if (fig == null) return;

            var state = argsList[0];

            var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
            if (infiniteRotationComponent == null)
            {
                infiniteRotationComponent = fig.AddComponent<InfiniteRotation>();
            }

            infiniteRotationComponent.Speed = 25.0f;
            infiniteRotationComponent.enabled = state == "on";

            QueryExecutor.Instance.RunCoroutine(PrimitiveManager.DelayPrimitive(1.0f));
        }

        public void Visibility(string args)
        {
            Debug.Log("Visibility: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var objs = ContextManager.Instance.GetAttribute<List<GameObject>>(argsList[1]);
            if (objs == null || objs.Count == 0) return;

            var state = argsList[0];

            foreach (var obj in objs)
            {
                if (obj.CompareTag(Tags.Figure))
                {
                    foreach (var child in obj.GetComponentsInChildren<Transform>())
                    {
                        var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
                        if (meshRenderer != null)
                        {
                            meshRenderer.enabled = state == "on";
                        }
                    }
                }
                else
                {
                    var meshRenderer = obj.gameObject.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = state == "on";
                    }
                }
            }

            QueryExecutor.Instance.RunCoroutine(PrimitiveManager.DelayPrimitive(1.0f));
        }

        public List<Action> CreateActions(string args)
        {
            Debug.Log("CreateActions: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var actionType = argsList[0];
            var refSpecified = argsList[1]; // always true for now
            var idList = ContextManager.Instance.GetAttribute<List<string>>(argsList[2]);

            var actionsList = new List<Action>();
            if (idList == null) return null;

            if (refSpecified.ToLower() == "yes")
            {
                var referenceId = idList[idList.Count - 1];

                for (var i = 0; i < idList.Count - 1; i++)
                {
                    actionsList.Add(new Action(actionType, new List<string> { idList[i], referenceId }));
                }
            }

            return actionsList;
        }

        public string CheckActionsValidity(string args)
        {
            Debug.Log("CheckActionsValidity: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var a = ContextManager.Instance.GetAttribute<List<Action>>(argsList[0]);
            var b = ContextManager.Instance.GetAttribute<List<JSONNode>>(argsList[1]);

            var parsedB = new List<Action>();

            foreach (var node in b)
            {
                foreach (var item in node)
                {
                    parsedB.Add(new Action(item.Key, new List<string>
                    {
                        item.Value[0],
                        item.Value[1]
                    }));
                }
            }

            for (var i = 0; i < a.Count; i++)
            {
                var actionA = a[i];
                var actionB = parsedB[i];

                if (actionA.Operation != actionB.Operation) return Constants.InvalidActions;

                if (actionA.Components[0] != actionB.Components[0] ||
                    actionA.Components[1] != actionB.Components[1]) return Constants.InvalidActions;
            }

            return Constants.ValidActions;
        }

        public void Attach(string args)
        {
            Debug.Log("Attach: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var valid = ContextManager.Instance.GetAttribute<string>(argsList[0]) == Constants.ValidActions;

            if (!valid) return;

            var actions = ContextManager.Instance.GetAttribute<List<Action>>(argsList[1]);

            var primitives = new List<IEnumerator>();
            foreach (var action in actions)
            {
                if (action.Operation == "attach")
                {
                    primitives.Add(PrimitiveManager.SimplePrimitive(() =>
                    {
                        UIManager.Instance.UpdateReplyText("Attaching " + action.Components[0] + " from " + action.Components[1]);
                    }));
                    primitives.AddRange(GetAttachPrimitives(action.Components[0], action.Components[1]));
                }
            }

            QueryExecutor.Instance.RunCoroutinesInSequence(primitives);
        }

        public void Detach(string args)
        {
            Debug.Log("Detach: " + args);
            var argsList = args.Split(Constants.ArgsSeparator);
            var valid = ContextManager.Instance.GetAttribute<string>(argsList[0]) == Constants.ValidActions;

            if (!valid) return;

            var actions = ContextManager.Instance.GetAttribute<List<Action>>(argsList[1]);

            var primitives = new List<IEnumerator>();
            foreach (var action in actions)
            {
                if (action.Operation == "detach")
                {
                    primitives.Add(PrimitiveManager.SimplePrimitive(() =>
                    {
                        UIManager.Instance.UpdateReplyText("Detaching " + action.Components[0] + " from " + action.Components[1]);
                    }));
                    primitives.AddRange(GetDetachPrimitives(action.Components[0], action.Components[1]));
                }
            }

            QueryExecutor.Instance.RunCoroutinesInSequence(primitives);
        }

        public static List<IEnumerator> GetAttachPrimitives(string nameA, string nameB)
        {
            var primitives = new List<IEnumerator>();

            var attachingObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
            var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);

            if (referenceObj == null)
            {
                var referenceObjReference = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, nameB);
                var currentFigure = AssetManager.Instance.GetCurrentFigure();

                referenceObj = AssetManager.Instance.CreateCloneObject(referenceObjReference, currentFigure,
                    Tags.CloneObject, false, true, true, true, true);
            }

            if (attachingObj == null)
            {
                var attachingObjReference = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, nameA);

                attachingObj = AssetManager.Instance.CreateCloneObject(attachingObjReference, referenceObj.transform.parent.gameObject,
                    Tags.CloneObject, false, true, true, true, true);
            }

            if (attachingObj.transform.childCount > 0)
            {
                primitives.Add(CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(attachingObj.transform.GetChild(0).name));
                primitives.Add(PrimitiveManager.DelayPrimitive(0.5f));
                primitives.Add(PrimitiveManager.Instance.GetAttachPrimitivesForChildren(nameA, nameB));
            }
            else
            {
                primitives.Add(PrimitiveManager.Instance.GetAttachPrimitivesForParent(nameA, nameB));
            }

            return primitives;
        }

        public static List<IEnumerator> GetDetachPrimitives(string nameA, string nameB)
        {
            var primitives = new List<IEnumerator>();

            var attachingObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
            var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);

            if (attachingObj.transform.childCount > 0)
            {
                primitives.Add(
                    CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(attachingObj.transform.GetChild(0).name));
                primitives.Add(PrimitiveManager.DelayPrimitive(0.5f));
                primitives.Add(
                    PrimitiveManager.Instance.GetDetachPrimitivesForChildren(attachingObj, referenceObj));
            }
            else
            {
                primitives.Add(PrimitiveManager.Instance.GetDetachPrimitivesForParent(attachingObj, referenceObj));
            }
            
            return primitives;
        }

        private void CloseLookFunctionality(List<GameObject> objs)
        {
            AssetManager.Instance.HideAllFigures();

            var grid = GameObject.FindWithTag(Tags.Grid);
            if (grid == null) return;

            foreach (var obj in objs)
            {
                AssetManager.Instance.CreateCloneObject(obj, grid);
            }

            QueryExecutor.Instance.RunCoroutine(CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(grid.name));
        }
    }
}