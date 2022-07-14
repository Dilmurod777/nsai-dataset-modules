using System.Collections;
using System.Collections.Generic;
using Custom;
using UnityEditor.VersionControl;
using UnityEngine;

public class PrimitiveManager : Singleton<PrimitiveManager>
{
	public delegate void FunctionDelegate();

	public static IEnumerator SimplePrimitive(FunctionDelegate callback)
	{
		callback();
		yield return null;
	}

	public IEnumerator CreateRotatePrimitives(GameObject objA)
	{
		var referenceObj = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var finalRotation = referenceObj.transform.rotation.eulerAngles;

		yield return StartCoroutine(Robot.Instance.SetRotateDuration(0.5f));
		yield return StartCoroutine(Robot.Instance.Rotate(objA, finalRotation));
		yield return StartCoroutine(Robot.Instance.ResetRotateDuration());
	}

	public IEnumerator CreateFromScatteredToRfmPrimitives(GameObject objA, GameObject objB)
	{
		var rfmReferenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
		var rfmReferenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);

		var rfmDiff = rfmReferenceObjA.transform.position - rfmReferenceObjB.transform.position;
		var finalPosition = objB.transform.position + rfmDiff;

		yield return StartCoroutine(Robot.Instance.Move(objA, finalPosition));
	}

	public IEnumerator SmoothInstall(GameObject objA, GameObject objB, string type)
	{
		GameObject referenceObjA, referenceObjB;

		if (type == "attach")
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objB.name);
		}
		else
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);
		}

		var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
		var finalPosition = objB.transform.position + rfmDiff;

		yield return StartCoroutine(Robot.Instance.Move(objA, finalPosition));
	}

	public IEnumerator SmoothScrew(GameObject objA, GameObject objB, Vector3 direction, string type)
	{
		GameObject referenceObjA, referenceObjB;

		if (type == "attach")
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objB.name);
		}
		else
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);
		}

		var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
		var finalPosition = objB.transform.position + rfmDiff;

		yield return StartCoroutine(Robot.Instance.MoveWithRotation(objA, finalPosition, direction));
	}

	public IEnumerator StepInstall(GameObject objA, GameObject objB, string type, int steps = 3)
	{
		GameObject referenceObjA, referenceObjB;

		if (type == "attach")
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objB.name);
		}
		else
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);
		}

		var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
		var delta = objB.transform.position + rfmDiff - objA.transform.position;

		for (var i = 0; i < steps; i++)
		{
			yield return StartCoroutine(Robot.Instance.Move(objA, objA.transform.position + (i + 1) * delta / steps));
			yield return StartCoroutine(Robot.Instance.Wait(0.25f));
		}
	}

	public IEnumerator StepScrew(GameObject objA, GameObject objB, Vector3 direction, string type, int steps = 3)
	{
		GameObject referenceObjA, referenceObjB;

		if (type == "attach")
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.IFM, objB.name);
		}
		else
		{
			referenceObjA = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objA.name);
			referenceObjB = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.RFM, objB.name);
		}

		var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
		var delta = objB.transform.position + rfmDiff - objA.transform.position;

		for (var i = 0; i < steps; i++)
		{
			yield return StartCoroutine(Robot.Instance.MoveWithRotation(objA, objA.transform.position + (i + 1) * delta / steps, direction));
			yield return StartCoroutine(Robot.Instance.Wait(0.25f));
		}
	}

	public IEnumerator ChangeObjectMaterialToInProgress(GameObject obj)
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

	public IEnumerator MakeObjectTransparent(GameObject obj, float finalAlpha = 0.0f, float seconds = 1.0f)
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

	public IEnumerator ResetObjectMaterial(GameObject obj)
	{
		var referenceObj = AssetManager.Instance.FindObjectInFigure(AssetManager.FigureType.Reference, obj.name);

		var referenceMaterials = referenceObj.GetComponent<MeshRenderer>().materials;
		var materials = obj.GetComponent<MeshRenderer>().materials;

		for (var i = 0; i < referenceMaterials.Length; i++)
		{
			materials[i] = referenceMaterials[i];
		}

		obj.GetComponent<MeshRenderer>().materials = materials;

		yield return null;
	}
}