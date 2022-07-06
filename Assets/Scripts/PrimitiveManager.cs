using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrimitiveManager
{
	public delegate void FunctionDelegate();

	public static IEnumerator SimplePrimitive(FunctionDelegate callback)
	{
		callback();
		yield return null;
	}

	public static List<IEnumerator> SmoothInstall(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		primitives.Add(RootManager.Robot.Move(objA, rfmFinalPosition));

		return primitives;
	}

	public static List<IEnumerator> SmoothScrew(GameObject objA, GameObject objB, Vector3 direction = default)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		primitives.Add(RootManager.Robot.MoveWithRotation(objA, rfmFinalPosition, direction));

		return primitives;
	}

	public static List<IEnumerator> StepInstall(GameObject objA, GameObject objB, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		primitives.Add(RootManager.Robot.Move(objA, rfmFinalPosition));

		return primitives;
	}

	public static List<IEnumerator> StepScrew(GameObject objA, GameObject objB, Vector3 direction = default, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);
		
		primitives.Add(RootManager.Robot.MoveWithRotation(objA, rfmFinalPosition, direction));

		return primitives;
	}
}