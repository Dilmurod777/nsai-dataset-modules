using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
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

		var referenceObj = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var finalRotation = referenceObj.transform.rotation.eulerAngles;

		primitives.Add(Robot.Instance.SetRotateDuration(0.5f));
		primitives.Add(Robot.Instance.Rotate(objA, finalRotation));
		primitives.Add(Robot.Instance.ResetRotateDuration());

		return primitives;
	}

	public static List<IEnumerator> CreateFromScatteredToRfmPrimitives(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		primitives.Add(SimplePrimitive(() =>
		{
			var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
			var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

			var rfmDiff = rfmReferenceObjA.transform.position - rfmReferenceObjB.transform.position;

			ContextManager.Instance.latestGameObjectPositions[objA.name] = ContextManager.Instance.latestGameObjectPositions[objB.name] + rfmDiff;
		}));
		primitives.Add(Robot.Instance.Move(objA));

		return primitives;
	}

	public static List<IEnumerator> SmoothUninstall(GameObject objA, GameObject objB)
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

	public static List<IEnumerator> SmoothUnscrew(GameObject objA, GameObject objB, Vector3 direction = default)
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

	public static List<IEnumerator> StepUninstall(GameObject objA, GameObject objB, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);

		var delta = rfmFinalPosition - objA.transform.position;
		for (var i = 0; i < steps; i++)
		{
			primitives.Add(Robot.Instance.Move(objA, objA.transform.position + (i + 1) * delta / steps));
			primitives.Add(Robot.Instance.Wait(0.25f));
		}

		return primitives;
	}

	public static List<IEnumerator> StepUnscrew(GameObject objA, GameObject objB, Vector3 direction = default, int steps = 3)
	{
		var primitives = new List<IEnumerator>();

		var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var previousParent = rfmReferenceObjA.transform.parent;
		rfmReferenceObjA.transform.SetParent(rfmReferenceObjB.transform);
		var rfmDiff = rfmReferenceObjA.transform.localPosition;
		var rfmFinalPosition = objB.transform.TransformPoint(rfmDiff);
		rfmReferenceObjA.transform.SetParent(previousParent);


		var delta = rfmFinalPosition - objA.transform.position;
		for (var i = 0; i < steps; i++)
		{
			primitives.Add(Robot.Instance.MoveWithRotation(objA, objA.transform.position + (i + 1) * delta / steps, direction));
			primitives.Add(Robot.Instance.Wait(0.25f));
		}

		return primitives;
	}

	public static List<IEnumerator> SmoothInstall(GameObject objA, GameObject objB)
	{
		var primitives = new List<IEnumerator>();

		primitives.Add(SimplePrimitive(() =>
		{
			var ifmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objA.name);
			var ifmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objB.name);

			var rfmDiff = ifmReferenceObjA.transform.position - ifmReferenceObjB.transform.position;

			ContextManager.Instance.latestGameObjectPositions[objA.name] = ContextManager.Instance.latestGameObjectPositions[objB.name] - rfmDiff;
		}));
		primitives.Add(Robot.Instance.Move(objA));

		return primitives;
	}

	public static IEnumerator ChangeObjectMaterialToInProgress(GameObject obj)
	{
		var oldMaterials = obj.GetComponent<MeshRenderer>().materials;
		var newMaterials = new Material[oldMaterials.Length];

		for (var i = 0; i < oldMaterials.Length; i++)
		{
			newMaterials[i] = AssetManager.Instance.inProgressMaterial;
		}

		obj.GetComponent<MeshRenderer>().materials = newMaterials;

		yield return null;
	}

	public static IEnumerator MakeObjectTransparent(GameObject obj, float finalAlpha = 0.0f, float seconds = 1.0f)
	{
		var delta = 0.0f;

		var oldMaterials = obj.GetComponent<MeshRenderer>().materials;

		while (delta <= seconds)
		{
			delta += Time.fixedDeltaTime;

			var newMaterials = new Material[oldMaterials.Length];
			for (var i = 0; i < newMaterials.Length; i++)
			{
				newMaterials[i] = new Material(AssetManager.Instance.tempMaterial);
				newMaterials[i].color = oldMaterials[i].color;
			}

			for (var i = 0; i < newMaterials.Length; i++)
			{
				var oldColor = newMaterials[i].color;
				var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.Lerp(oldColor.a, 0.0f, delta / seconds));
				newMaterials[i].color = newColor;
			}

			obj.GetComponent<MeshRenderer>().materials = newMaterials;

			yield return null;
		}

		yield return null;
	}
}