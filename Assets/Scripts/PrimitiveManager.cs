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

		var referenceObj = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objA.name);
		var finalRotation = referenceObj.transform.eulerAngles;

		primitives.Add(Robot.Instance.SetRotateDuration(0.5f));
		primitives.Add(Robot.Instance.Rotate(objA, finalRotation));
		primitives.Add(Robot.Instance.ResetRotateDuration());

		return primitives;
	}

	public static List<IEnumerator> CreateRfmToScatteredMovePrimitives(GameObject objA)
	{
		var primitives = new List<IEnumerator>();

		var figureStaticComponent = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.Current, objA.name);
		var ifmStaticComponent = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.Scattered, objA.name);
		;

		var ifmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.Scattered, objA.name);
		var ifmFinalPosition = ifmReferenceObjA.transform.position +
		                       new Vector3(0, 0, figureStaticComponent.transform.position.z - ifmStaticComponent.transform.position.z);

		// var ifmReferenceObjA = RootManager.Instance.assetManager.FindObjectInFigure(AssetManager.FigureType.Scattered, objA.name);
		// var ifmFinalPosition = ifmReferenceObjA.transform.position;

		primitives.Add(Robot.Instance.SetMoveDuration(0.5f));
		primitives.Add(Robot.Instance.Move(objA, ifmFinalPosition));
		primitives.Add(Robot.Instance.ResetMoveDuration());

		return primitives;
	}

	public static List<IEnumerator> SmoothInstall(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		primitives.Add(Robot.Instance.Move(objA, rfmFinalPosition));

		return primitives;
	}

	public static List<IEnumerator> SmoothScrew(GameObject objA, GameObject objB, Vector3 direction = default)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		primitives.Add(Robot.Instance.MoveWithRotation(objA, rfmFinalPosition, direction));

		return primitives;
	}

	public static List<IEnumerator> StepInstall(GameObject objA, GameObject objB, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		primitives.Add(Robot.Instance.Move(objA, rfmFinalPosition));

		return primitives;
	}

	public static List<IEnumerator> StepScrew(GameObject objA, GameObject objB, Vector3 direction = default, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		primitives.Add(Robot.Instance.MoveWithRotation(objA, rfmFinalPosition, direction));

		return primitives;
	}
}