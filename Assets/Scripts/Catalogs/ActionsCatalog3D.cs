using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Custom;
using Instances;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;

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
		void CreateActions(string args);
		string CheckActionsValidity(string args);
	}

	public class ActionsCatalog3D : IActionsCatalog3DInterface
	{
		// private IEnumerator Sequence(List<IEnumerator> coroutines, float delay = 0.0f)
		// {
		// 	yield return new WaitForSeconds(delay);
		// 	foreach (var c in coroutines) yield return StartCoroutine(c);
		// }
		//
		// private static void DestroyCloneObjects()
		// {
		// 	var cloneObjects = GameObject.FindGameObjectsWithTag("CloneObject");
		//
		// 	foreach (var cloneObj in cloneObjects)
		// 	{
		// 		Destroy(cloneObj);
		// 	}
		// }

		public List<GameObject> Filter3DAttr(string args)
		{
			Debug.Log("Filter3DAttr: " + args);
			var argsList = args.Split(Constants.ArgsSeparator);
			var attrName = argsList[0];
			var prev = ContextManager.Instance.HasAttribute(argsList[1]) ? ContextManager.Instance.GetAttribute<System.Object>(argsList[1]) : argsList[1];
			var parent = new List<GameObject>();
			if (argsList[2] != "root3D")
			{
				parent = ContextManager.Instance.GetAttribute<List<GameObject>>(argsList[2]);
			}

			var allObjects = new List<GameObject>();
			switch (attrName)
			{
				case "name":
					var ids = new List<string>();

					if (prev is string) ids.Add((string) prev);
					if (prev is IList) ids.AddRange((IList<string>) prev);

					var objects = FindObjectsWithIds(ids, parent);
					var figures = FindFigureWithId(ids, parent);

					allObjects.AddRange(objects);
					allObjects.AddRange(figures);
					break;
				case "type":
					if (prev == "figure" && parent != null)
					{
						var currentTask = ContextManager.Instance.CurrentTask;
						var taskType = ContextManager.GetTaskType(currentTask);
						var plainFigureName = Helpers.GetCurrentFigurePlainName();
						var figureName = taskType == ContextManager.TaskType.Installation ? plainFigureName + "-Installation" : plainFigureName + "-Removal";

						foreach (var obj in parent)
						{
							if (obj.CompareTag("Figure") && obj.name == figureName)
							{
								allObjects.Add(obj);
							}
						}
					}

					break;
			}

			return allObjects;
		}

		// find object with part of name
		private static List<GameObject> FindObjectsWithIds(List<string> ids, List<GameObject> parentObjects)
		{
			var allObjects = parentObjects;
			if (parentObjects == null || parentObjects.Count == 0)
			{
				allObjects = GameObject.FindGameObjectsWithTag("Object").ToList();
			}

			var currentTask = ContextManager.Instance.CurrentTask;
			var taskType = ContextManager.GetTaskType(currentTask);
			var plainFigureName = Helpers.GetCurrentFigurePlainName();
			var figureName = taskType == ContextManager.TaskType.Installation ? plainFigureName + "-Installation" : plainFigureName + "-Removal";

			var foundObs = new List<GameObject>();

			foreach (var id in ids)
			foreach (var obj in allObjects)
				if (obj.name.Contains(id) && obj.transform.parent.name.Equals(figureName))
					foundObs.Add(obj);

			return foundObs;
		}

		private static List<GameObject> FindFigureWithId(List<string> ids, List<GameObject> parentFigures)
		{
			var allFigures = parentFigures;
			if (parentFigures == null || parentFigures.Count == 0)
			{
				allFigures = GameObject.FindGameObjectsWithTag("Figure").ToList();
			}

			var foundFigs = new List<GameObject>();

			var currentTask = ContextManager.Instance.CurrentTask;
			var taskType = ContextManager.GetTaskType(currentTask);

			foreach (var fig in allFigures)
			foreach (var id in ids)
			{
				var plainFigureName = Helpers.GetFigurePlainName(id);
				var figureName = taskType == ContextManager.TaskType.Installation ? plainFigureName + "-Installation" : plainFigureName + "-Removal";

				if (fig.name == figureName)
					foundFigs.Add(fig);
			}

			return foundFigs;
		}

		public void Rotate(string args)
		{
			Debug.Log("Rotate: " + args);
			var argsList = args.Split(Constants.ArgsSeparator);
			var obj = ContextManager.Instance.GetAttribute<GameObject>(argsList[0]);
			if (obj == null) return;

			var restArgs = ContextManager.Instance.GetAttribute<List<string>>(argsList[1]);
			if (restArgs != null)
			{
				var degree = float.Parse(restArgs[0]);
				var axisRegex = Regex.Match(ContextManager.Instance.CurrentQuery.Title, @"[XYZ] axis").Value;

				var rotation = obj.transform.rotation;
				var axis = axisRegex.Split(' ')[0];
				var rotationX = axis == "X" ? degree : 0;
				var rotationY = axis == "Y" ? degree : 0;
				var rotationZ = axis == "Z" ? degree : 0;

				var newRotation = rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
				QueryExecutor.Instance.RunCoroutine(Robot.Instance.Rotate(obj, newRotation.eulerAngles));
			}
		}

		public void Scale(string args)
		{
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

			QueryExecutor.Instance.StartCoroutine(Robot.Instance.Scale(obj, finalScale));
		}

		public void Reset(string args)
		{
		}

		public void Highlight(string args)
		{
			var argsList = args.Split(Constants.ArgsSeparator);
			var state = argsList[0];

			var objs = ContextManager.Instance.GetAttribute<List<GameObject>>(argsList[1]);
			if (objs == null || objs.Count == 0) return;

			foreach (var obj in objs)
			{
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
		}

		public void ShowSide(string args)
		{
			var argsList = args.Split(Constants.ArgsSeparator);
			var obj = ContextManager.Instance.GetAttribute<GameObject>(argsList[1]);
			if (obj == null) return;

			var figureSide = argsList[0];

			var sideRotation = figureSide switch
			{
				"front" => Quaternion.Euler(0, -90, -45),
				"back" => Quaternion.Euler(0, 90, 0),
				"right" => Quaternion.Euler(0, 0, 0),
				"left" => Quaternion.Euler(0, 180, 0),
				"top" => Quaternion.Euler(-45, 0, 0),
				"bottom" => Quaternion.Euler(135, 0, 0),
				_ => Quaternion.Euler(0, 0, 0)
			};

			// var coroutines = new List<IEnumerator>
			// {
			// RotateObjectCoroutine(obj, sideRotation, 1.0f)
			// };

			QueryExecutor.Instance.RunCoroutine(Robot.Instance.Rotate(obj, sideRotation.eulerAngles));

			// StartCoroutine(Sequence(coroutines));
		}

		public void SideBySideLook(string args)
		{
			var argsList = args.Split(' ');
			var fig = ContextManager.Instance.GetAttribute<GameObject>(argsList[0]);
			if (fig == null) return;

			var objs = new List<GameObject>();
			for (var i = 0; i < fig.transform.childCount; i++)
			{
				objs.Add(fig.transform.GetChild(i).gameObject);
			}

			CloseLookFunctionality(objs);

			var objNames = objs.Select(obj => obj.name).ToList();
		}

		public void CloseLook(string args)
		{
			var argsList = args.Split(Constants.ArgsSeparator);
			var objs = ContextManager.Instance.GetAttribute<List<GameObject>>(argsList[0]);
			if (objs == null || objs.Count == 0) return;

			CloseLookFunctionality(objs);

			var objNames = objs.Select(obj => obj.name).ToList();
		}

		public void Animate(string args)
		{
			var argsList = args.Split(Constants.ArgsSeparator);
			var fig = ContextManager.Instance.GetAttribute<GameObject>(argsList[1]);
			if (fig == null) return;

			var state = argsList[0];
			// Attributes attributes = Context.Instance.InitialAttributes[fig.name];
			//
			// void StartAnimatingFigure()
			// {
			// 	var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
			// 	if (infiniteRotationComponent == null)
			// 	{
			// 		infiniteRotationComponent = fig.AddComponent<InfiniteRotation>();
			// 		infiniteRotationComponent.SetSpeed(25.0f);
			// 	}
			// }
			//
			// void StopAnimatingFigure()
			// {
			// 	var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
			// 	if (infiniteRotationComponent != null)
			// 	{
			// 		Destroy(infiniteRotationComponent);
			// 	}
			//
			// 	StartCoroutine(ResetObjectCoroutine(fig, attributes, 1.0f));
			// }

			// var infiniteRotationComponent = fig.GetComponent<InfiniteRotation>();
			// if (infiniteRotationComponent == null && state == "on")
			// {
			// 	StartCoroutine(ResetObjectCoroutine(fig, attributes, 1.0f));
			// }
			//
			// StartCoroutine(state == "on"
			// 	? DelayCoroutine(1.0f, StartAnimatingFigure)
			// 	: DelayCoroutine(1.0f, StopAnimatingFigure));
		}

		public void Visibility(string args)
		{
			var argsList = args.Split(Constants.ArgsSeparator);
			var objs = ContextManager.Instance.GetAttribute<List<GameObject>>(argsList[1]);
			if (objs == null || objs.Count == 0) return;

			var state = argsList[0];

			foreach (var obj in objs)
			{
				obj.GetComponent<MeshRenderer>().enabled = state == "on";
			}

			var objNames = objs.Select(obj => obj.name).ToList();
		}

		public void CreateActions(string args)
		{
			var argsList = args.Split(Constants.ArgsSeparator);
			var actionType = argsList[0];
			var refSpecified = argsList[1]; // always true for now
			var idList = ContextManager.Instance.GetAttribute<List<string>>(argsList[2]);


			var actionsList = new List<Action>();
			if (idList == null) return;

			if (refSpecified == "yes")
			{
				var referenceId = idList[idList.Count - 1];

				// for (var i = 0; i < idList.Count - 1; i++)
				// {
				// actionsList.Add(new Action
				// {
				// 	"attach",
				// 	new List<string>{idList[i], referenceId}
				// });
				// }
			}
		}

		public string CheckActionsValidity(string args)
		{
			// var argsList = args.Split(General.ArgsSeparator);
			// List<Action> a = ContextManager.Instance.GetAttribute(argsList[0]);
			// List<JSONNode> b = ContextManager.Instance.GetAttribute(argsList[1]);
			//
			// var parsedB = new List<Action>();
			//
			// foreach (var node in b)
			// {
			// 	foreach (var item in node)
			// 	{
			// parsedB.Add(new Action
			// {
			// Name = item.Key,
			// Components = new List<string>
			// {
			// 	"[" + item.Value[0] + "]",
			// 	"[" + item.Value[1] + "]"
			// }
			// });
			// 	}
			// }
			//
			// // comparison logic
			// if (a.Count != parsedB.Count) return "no";
			//
			// for (var i = 0; i < a.Count; i++)
			// {
			// 	var actionA = a[i];
			// 	var actionB = parsedB[i];

			// if (actionA.Name != actionB.Name) return "no";
			//
			// if (actionA.Components[0] != actionB.Components[0] ||
			//     actionA.Components[1] != actionB.Components[1]) return "no";
			// }
			//
			// return "yes";

			return "";
		}

		public void Attach(string args)
		{
			// Vector3 delta;
			// ObjectMeta.RotationAxisEnum rotationAxis;
			// const int steps = 3;
			//
			// var argsList = args.Split(General.ArgsSeparator);
			// var valid = ContextManager.Instance.GetAttribute(argsList[0]) == "yes";
			//
			// if (!valid) return null;
			//
			// List<Action> actionsList = ContextManager.Instance.GetAttribute(argsList[1]);
			//
			// var figureID = ContextManager.Instance.Instance.CurrentFigureID;
			// var figure = GameObject.Find(figureID);
			// var figureRfmName = figureID + "-RFM";
			// var figureIfmName = figureID + "-IFM";
			//
			// var rfm = GameObject.Find(figureRfmName);
			// var ifm = GameObject.Find(figureIfmName);
			//
			// var routines = new List<IEnumerator>();
			// var attachingObjs = new List<string>();
			// var referenceObjs = new List<string>();
			//
			// foreach (var action in actionsList)
			// {
			// 	// var objA = HelperFunctions.FindObjectInFigure(figure, action.Components[0]);
			// 	// var objB = HelperFunctions.FindObjectInFigure(figure, action.Components[1]);
			//
			// 	attachingObjs.Add(objA.name);
			// 	referenceObjs.Add(objB.name);
			// 	
			// 	FocusObject(objA, objB);
			//
			// 	var rfmReferenceObjA = HelperFunctions.FindObjectInFigure(rfm, objA.name);
			// 	var rfmReferenceObjB = HelperFunctions.FindObjectInFigure(rfm, objB.name);
			// 	var ifmReferenceObjA = HelperFunctions.FindObjectInFigure(ifm, objA.name);
			// 	var ifmReferenceObjB = HelperFunctions.FindObjectInFigure(ifm, objB.name);
			//
			// 	rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform;
			// 	var diff = rfmReferenceObjA.transform.localPosition;
			// 	var rfmFinalPosition = objB.transform.TransformPoint(diff);
			// 	rfmReferenceObjA.transform.parent = rfmReferenceObjB.transform.parent;
			//
			// 	ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform;
			// 	diff = ifmReferenceObjA.transform.localPosition;
			// 	var ifmFinalPosition = objB.transform.TransformPoint(diff);
			// 	ifmReferenceObjA.transform.parent = ifmReferenceObjB.transform.parent;
			//
			// 	routines.Add(RotateObjectCoroutine(objA, objB.transform.rotation, 0.5f));
			// 	routines.Add(DelayCoroutine(0.5f));
			// 	routines.Add(AdjustStructureCoroutine(objA, objB));
			// 	routines.Add(MoveObjectCoroutine(objA, rfmFinalPosition, 0.5f));
			// 	routines.Add(DelayCoroutine(0.5f));
			//
			// 	var objAObjectMeta = objA.GetComponent<ObjectMeta>();
			// 	var attachType = GeneralConstants.AttachTypes.SmoothInstall;
			// 	if (objAObjectMeta != null)
			// 	{
			// 		attachType = objAObjectMeta.AttachType;
			// 	}
			// 	
			// 	switch (attachType)
			// 	{
			// 		case GeneralConstants.AttachTypes.SmoothInstall:
			// 			routines.Add(MoveObjectCoroutine(objA, ifmFinalPosition, 1.0f));
			// 			break;
			// 		case GeneralConstants.AttachTypes.StepInstall:
			// 			delta = ifmFinalPosition - rfmFinalPosition;
			// 		
			// 			for (var i = 1; i <= steps; i++)
			// 			{
			// 				routines.Add(MoveObjectCoroutine(objA, rfmFinalPosition + delta * i / steps, 0.5f));
			// 				routines.Add(DelayCoroutine(0.3f));
			// 			}
			//
			// 			break;
			// 		case GeneralConstants.AttachTypes.SmoothScrew:
			// 			rotationAxis = objA.GetComponent<ObjectMeta>().AttachRotationAxis;
			// 		
			// 			routines.Add(MoveObjectWithRotationCoroutine(objA, ifmFinalPosition, 2.0f, GetScrewVector(rotationAxis)));
			// 			break;
			// 		case GeneralConstants.AttachTypes.StepScrew:
			// 			delta = ifmFinalPosition - rfmFinalPosition;
			// 			rotationAxis = objA.GetComponent<ObjectMeta>().AttachRotationAxis;
			//
			// 			for (var i = 1; i <= steps; i++)
			// 			{
			// 				routines.Add(MoveObjectWithRotationCoroutine(objA, rfmFinalPosition + delta * i / steps, 0.5f, GetScrewVector(rotationAxis)));
			// 				routines.Add(DelayCoroutine(0.3f));
			// 			}
			//
			// 			break;
			// 		default:
			// 			routines.Add(MoveObjectCoroutine(objA, ifmFinalPosition, 1.0f));
			// 			break;
			// 	}
			// 	
			// 	routines.Add(DelayCoroutine(0.5f));
			// }
			//
			// StartCoroutine(Sequence(routines, 1.0f));
			//
			// var objs = new List<string>();
			// objs.AddRange(attachingObjs.Distinct().ToList());
			// objs.AddRange(referenceObjs.Distinct().ToList());

			// return new Response(new Dictionary<string, dynamic>
			// {
			// 	{"name", "attach"}
			// }, new List<string>(), new Dictionary<string, dynamic>());
		}

		private static void FocusObject(GameObject objA, GameObject objB)
		{
			// var objAPosition = objA.transform.position;
			// var objBPosition = objB.transform.position;
			// var middlePosition = (objAPosition + objBPosition) / 2;
			//
			// Context.Instance.Camera.transform.LookAt(middlePosition);
		}

		private static Vector3 GetScrewVector(ObjectMeta.RotationAxisEnum rotationAxis)
		{
			switch (rotationAxis)
			{
				case ObjectMeta.RotationAxisEnum.X:
					return Vector3.right;
				case ObjectMeta.RotationAxisEnum.NegX:
					return Vector3.left;
				case ObjectMeta.RotationAxisEnum.Y:
					return Vector3.up;
				case ObjectMeta.RotationAxisEnum.NegY:
					return Vector3.down;
				case ObjectMeta.RotationAxisEnum.Z:
					return Vector3.forward;
				case ObjectMeta.RotationAxisEnum.NegZ:
					return Vector3.back;
				default:
					return Vector3.forward;
			}
		}

		private void CloseLookFunctionality(List<GameObject> objs)
		{
			// var parentFigure = objs[0].transform.parent.gameObject;
			// var cloneObjects = new List<GameObject>();
			//
			// void CreateCloneObjects()
			// {
			// 	for (var i = 0; i < parentFigure.transform.childCount; i++)
			// 	{
			// 		var child = parentFigure.transform.GetChild(i);
			// 		child.GetComponent<MeshRenderer>().enabled = false;
			// 	}
			//
			// 	foreach (var obj in objs)
			// 	{
			// 		var cloneObject = Instantiate(obj);
			// 		cloneObject.tag = "CloneObject";
			// 		cloneObject.GetComponent<MeshRenderer>().enabled = true;
			// 		cloneObject.transform.rotation = parentFigure.transform.rotation;
			// 		cloneObject.transform.position = parentFigure.transform.position;
			// 		cloneObjects.Add(cloneObject);
			// 	}
			// }
			//
			// void MoveCloneObjects()
			// {
			// 	var coroutines = new List<IEnumerator>();
			// 	var width = Math.Max(20, 5 * cloneObjects.Count);
			// 	for (var i = 0; i < cloneObjects.Count; i++)
			// 	{
			// 		coroutines.Add(MoveObjectCoroutine(cloneObjects[i],
			// 			new Vector3((i + 1) * width / (cloneObjects.Count + 1) - width / 2, 5.0f, 0), 0.2f));
			// 	}
			//
			// 	StartCoroutine(Sequence(coroutines));
			// }

			// Attributes attributes = Context.Instance.InitialAttributes[parentFigure.name];
			// var coroutines = new List<IEnumerator>
			// {
			// 	ResetObjectCoroutine(parentFigure, attributes, 0.2f),
			// 	DelayCoroutine(0.2f, CreateCloneObjects),
			// 	DelayCoroutine(0.2f, MoveCloneObjects)
			// };

			// StartCoroutine(Sequence(coroutines));
		}

		// private static IEnumerator ResetObjectCoroutine(GameObject obj, Attributes attributes, float duration)
		// {
		// 	if (ScriptExecutor.IsInAction) yield break;
		// 	ScriptExecutor.IsInAction = true;
		//
		// 	var currentRot = obj.transform.rotation;
		// 	var currentScale = obj.transform.localScale;
		//
		// 	var infiniteRotationComponent = obj.GetComponent<InfiniteRotation>();
		// 	if (infiniteRotationComponent != null)
		// 	{
		// 		Destroy(infiniteRotationComponent);
		// 	}
		//
		// 	DestroyCloneObjects();
		//
		// 	foreach (var o in obj.transform.GetComponentsInChildren<Transform>())
		// 	{
		// 		var oTransform = o.transform;
		// 		var meshRenderer = o.GetComponent<MeshRenderer>();
		// 		if (meshRenderer != null)
		// 		{
		// 			o.GetComponent<MeshRenderer>().enabled = true;
		// 		}
		//
		// 		if (Context.Instance.InitialAttributes.ContainsKey(obj.name + GeneralConstants.ArgsSeparator + o.name))
		// 		{
		// 			var attr = Context.Instance.InitialAttributes[obj.name + GeneralConstants.ArgsSeparator + o.name];
		// 			oTransform.rotation = attr.Rotation;
		// 			oTransform.position = attr.Position;
		// 			oTransform.localScale = attr.Scale;
		// 		}
		//
		// 		o.transform.parent = obj.transform;
		// 	}
		//
		// 	float counter = 0;
		// 	while (counter < 1)
		// 	{
		// 		counter += Time.deltaTime / duration;
		//
		// 		obj.transform.rotation = Quaternion.Lerp(currentRot, attributes.Rotation, counter);
		// 		obj.transform.localScale = Vector3.Lerp(currentScale, attributes.Scale, counter);
		// 		obj.transform.position = Vector3.Lerp(obj.transform.position, attributes.Position, counter);
		//
		// 		yield return null;
		// 	}
		// }

		private static IEnumerator AdjustStructureCoroutine(GameObject objA, GameObject objB, float duration = 0.1f)
		{
			objA.transform.parent = objB.transform;

			float elapsedTime = 0;
			while (elapsedTime <= duration)
			{
				objA.transform.parent = objB.transform;
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}

		// smoothly move object
		private static IEnumerator MoveObjectCoroutine(GameObject obj, Vector3 finalPosition, float duration)
		{
			// if (ScriptExecutor.IsInAction) yield break;
			// ScriptExecutor.IsInAction = true;
			//
			// float counter = 0;
			// var currentPos = obj.transform.position;
			// while (counter < 1)
			// {
			// 	counter += Time.deltaTime / duration;
			//
			// 	obj.transform.position = Vector3.Lerp(currentPos, finalPosition, counter);
			// 	yield return new WaitForSeconds(0.0001f);
			// }

			// ScriptExecutor.IsInAction = false;
			yield return null;
		}

		private static IEnumerator MoveObjectWithRotationCoroutine(GameObject obj, Vector3 finalPosition, float duration, Vector3 direction = default)
		{
			// if (ScriptExecutor.IsInAction) yield break;
			// ScriptExecutor.IsInAction = true;
			//
			// var infiniteRotationComponents = obj.GetComponents<InfiniteRotation>();
			// InfiniteRotation infiniteRotationComponent;
			// if (infiniteRotationComponents.Length == 0)
			// {
			// 	infiniteRotationComponent = obj.AddComponent<InfiniteRotation>();
			// }
			// else if (infiniteRotationComponents.Length > 1)
			// {
			// 	for (var i = 1; i < infiniteRotationComponents.Length; i++)
			// 	{
			// 		Destroy(infiniteRotationComponents[i]);
			// 	}
			//
			// 	infiniteRotationComponent = infiniteRotationComponents[0];
			// }
			// else
			// {
			// 	infiniteRotationComponent = infiniteRotationComponents[0];
			// }
			//
			//
			// infiniteRotationComponent.SetSpeed(150);
			// infiniteRotationComponent.SetDirection(direction);
			//
			// var currentPos = obj.transform.position;
			// float counter = 0;
			// while (counter < 1)
			// {
			// 	counter += Time.deltaTime / duration;
			// 	obj.transform.position = Vector3.Lerp(currentPos, finalPosition, counter);
			// 	yield return null;
			// }
			//
			// Destroy(infiniteRotationComponent);
			//
			// ScriptExecutor.IsInAction = false;

			yield return null;
		}

		// smoothly change rotation of camera
		private static IEnumerator RotateObjectCoroutine(dynamic obj, Quaternion finalRotation, float duration)
		{
			// if (ScriptExecutor.IsInAction) yield break;
			// ScriptExecutor.IsInAction = true;
			//
			// var currentRot = obj.transform.rotation;
			// float counter = 0;
			// while (counter < 1)
			// {
			// 	counter += Time.deltaTime / duration;
			//
			// 	obj.transform.rotation = Quaternion.Slerp(currentRot, finalRotation, counter);
			// 	yield return null;
			// }
			//
			// ScriptExecutor.IsInAction = false;

			yield return null;
		}

		private static IEnumerator ScaleObjectCoroutine(dynamic obj, Vector3 finalScale, float duration)
		{
			// if (ScriptExecutor.IsInAction) yield break;
			// ScriptExecutor.IsInAction = true;
			//
			// var currentScale = obj.transform.localScale;
			// float counter = 0;
			// while (counter < 1)
			// {
			// 	counter += Time.deltaTime / duration;
			//
			// 	obj.transform.localScale = Vector3.Lerp(currentScale, finalScale, counter);
			// 	yield return null;
			// }
			//
			// ScriptExecutor.IsInAction = false;
			yield return null;
		}

		// smoothly change FoV of camera
		private static IEnumerator ChangeFieldOfViewByValueCoroutine(Camera camera, float finalFoV, float duration)
		{
			// if (ScriptExecutor.IsInAction) yield break;
			//
			// ScriptExecutor.IsInAction = true;
			//
			// var currentFOV = camera.fieldOfView;
			// float counter = 0;
			// while (counter < duration)
			// {
			// 	counter += Time.deltaTime;
			//
			// 	camera.fieldOfView = Mathf.Lerp(currentFOV, finalFoV, counter / duration);
			// 	yield return null;
			// }
			//
			// ScriptExecutor.IsInAction = false;
			yield return null;
		}
	}
}