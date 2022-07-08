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

	public static List<IEnumerator> CreateRotatePrimitives(GameObject objA)
	{
		var primitives = new List<IEnumerator>();

		var referenceObj = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.IFM, objA.name);
		var finalRotation = referenceObj.transform.eulerAngles;

		primitives.Add(RootManager.Robot.SetRotateDuration(0.5f));
		primitives.Add(RootManager.Robot.Rotate(objA, finalRotation));
		primitives.Add(RootManager.Robot.ResetRotateDuration());

		return primitives;
	}

	public static List<IEnumerator> CreateRfmToScatteredMovePrimitives(GameObject objA)
	{
		var primitives = new List<IEnumerator>();

		var figureStaticComponent = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.Current, objA.name);
		var ifmStaticComponent = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.Scattered, objA.name);
		;

		var ifmReferenceObjA = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.Scattered, objA.name);
		var ifmFinalPosition = ifmReferenceObjA.transform.position +
		                       new Vector3(0, 0, figureStaticComponent.transform.position.z - ifmStaticComponent.transform.position.z);

		// var ifmReferenceObjA = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.Scattered, objA.name);
		// var ifmFinalPosition = ifmReferenceObjA.transform.position;

		primitives.Add(RootManager.Robot.SetMoveDuration(0.5f));
		primitives.Add(RootManager.Robot.Move(objA, ifmFinalPosition));
		primitives.Add(RootManager.Robot.ResetMoveDuration());

		return primitives;
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