using System.Collections;
using System.Collections.Generic;
using Custom;
using Instances;
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

	public static IEnumerator DelayPrimitive(float seconds)
	{
		yield return new WaitForSeconds(seconds);
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

		var initialPosition = objA.transform.position;
		var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
		var delta = objB.transform.position + rfmDiff - initialPosition;
		var newPositions = new Vector3[steps];

		for (var i = 0; i < steps; i++)
		{
			newPositions[i] = initialPosition + (i + 1) * delta / steps;
		}

		for (var i = 0; i < steps; i++)
		{
			yield return StartCoroutine(Robot.Instance.MoveWithRotation(objA, newPositions[i], direction));
			yield return StartCoroutine(Robot.Instance.Wait(0.25f));
		}
	}

	public void ChangeObjectMaterialToInProgress(GameObject obj)
	{
		var oldMaterials = obj.GetComponent<MeshRenderer>().materials;
		var newMaterials = new Material[oldMaterials.Length];

		for (var i = 0; i < oldMaterials.Length; i++)
		{
			newMaterials[i] = AssetManager.Instance.inProgressMaterial;
		}

		obj.GetComponent<MeshRenderer>().materials = newMaterials;
	}

	public IEnumerator ChangeObjectMaterialToInProgressCoroutine(GameObject obj)
	{
		ChangeObjectMaterialToInProgress(obj);
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

	public IEnumerator GetDetachPrimitivesForParent(GameObject attachingObj, GameObject referenceObj)
	{
		var objectMeta = attachingObj.GetComponent<ObjectMeta>();

		if (objectMeta.status == ObjectMeta.Status.Dettached)
		{
			yield return DelayPrimitive(1.0f);
			yield break;
		}

		var text = "";
		const string delimiter = " and ";

		text += objectMeta.attachType switch
		{
			ObjectMeta.AttachTypes.SmoothInstall => "Smooth Uninstall ",
			ObjectMeta.AttachTypes.StepInstall => "Step Uninstall ",
			ObjectMeta.AttachTypes.SmoothScrew => "Smooth Unscrew ",
			ObjectMeta.AttachTypes.StepScrew => "Step Unscrew ",
			_ => "Smooth Uninstall "
		};

		text += attachingObj.name + delimiter + referenceObj.name;
		yield return SimplePrimitive(() => { UIManager.UpdateReply(text); });

		yield return CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(attachingObj);
		yield return DelayPrimitive(0.5f);

		yield return Instance.ChangeObjectMaterialToInProgressCoroutine(attachingObj);
		yield return DelayPrimitive(2f);

		var rotationAxis = objectMeta.attachRotationAxis;

		var attachRotationVector = rotationAxis switch
		{
			ObjectMeta.RotationAxisEnum.X => Vector3.right,
			ObjectMeta.RotationAxisEnum.NegX => Vector3.left,
			ObjectMeta.RotationAxisEnum.Y => Vector3.up,
			ObjectMeta.RotationAxisEnum.NegY => Vector3.down,
			ObjectMeta.RotationAxisEnum.Z => Vector3.forward,
			ObjectMeta.RotationAxisEnum.NegZ => Vector3.back,
			_ => Vector3.forward
		};

		switch (objectMeta.attachType)
		{
			case ObjectMeta.AttachTypes.SmoothInstall:
				yield return Instance.SmoothInstall(attachingObj, referenceObj, "detach");
				break;
			case ObjectMeta.AttachTypes.StepInstall:
				yield return Instance.StepInstall(attachingObj, referenceObj, "detach");
				break;
			case ObjectMeta.AttachTypes.SmoothScrew:
				yield return Instance.SmoothScrew(attachingObj, referenceObj, attachRotationVector, "detach");
				break;
			case ObjectMeta.AttachTypes.StepScrew:
				yield return Instance.StepScrew(attachingObj, referenceObj, attachRotationVector, "detach");
				break;
			default:
				yield return Instance.SmoothInstall(attachingObj, referenceObj, "detach");
				break;
		}

		yield return DelayPrimitive(0.5f);
		yield return Instance.MakeObjectTransparent(attachingObj);
		yield return SimplePrimitive(() => { objectMeta.status = ObjectMeta.Status.Dettached; });
	}

	public IEnumerator GetDetachPrimitivesForChildren(GameObject attachingObj, GameObject referenceObj)
	{
		yield return SimplePrimitive(() =>
		{
			for (var i = 0; i < attachingObj.transform.childCount; i++)
			{
				var child = attachingObj.transform.GetChild(i).gameObject;
				var objectMeta = child.GetComponent<ObjectMeta>();

				if (objectMeta.status == ObjectMeta.Status.Dettached)
				{
					continue;
				}

				ChangeObjectMaterialToInProgress(attachingObj.transform.GetChild(i).gameObject);
			}
		});
		yield return StartCoroutine(DelayPrimitive(1.5f));


		yield return SimplePrimitive(() =>
		{
			for (var i = 0; i < attachingObj.transform.childCount; i++)
			{
				var child = attachingObj.transform.GetChild(i).gameObject;
				var objectMeta = child.GetComponent<ObjectMeta>();

				if (objectMeta.status == ObjectMeta.Status.Dettached)
				{
					continue;
				}

				var rotationAxis = objectMeta.attachRotationAxis;

				var attachRotationVector = rotationAxis switch
				{
					ObjectMeta.RotationAxisEnum.X => Vector3.right,
					ObjectMeta.RotationAxisEnum.NegX => Vector3.left,
					ObjectMeta.RotationAxisEnum.Y => Vector3.up,
					ObjectMeta.RotationAxisEnum.NegY => Vector3.down,
					ObjectMeta.RotationAxisEnum.Z => Vector3.forward,
					ObjectMeta.RotationAxisEnum.NegZ => Vector3.back,
					_ => Vector3.forward
				};

				switch (objectMeta.attachType)
				{
					case ObjectMeta.AttachTypes.SmoothInstall:
						StartCoroutine(Instance.SmoothInstall(child, referenceObj, "detach"));
						break;
					case ObjectMeta.AttachTypes.StepInstall:
						StartCoroutine(Instance.StepInstall(child, referenceObj, "detach"));
						break;
					case ObjectMeta.AttachTypes.SmoothScrew:
						StartCoroutine(Instance.SmoothScrew(child, referenceObj, attachRotationVector, "detach"));
						break;
					case ObjectMeta.AttachTypes.StepScrew:
						StartCoroutine(Instance.StepScrew(child, referenceObj, attachRotationVector, "detach"));
						break;
					default:
						StartCoroutine(Instance.SmoothInstall(child, referenceObj, "detach"));
						break;
				}
			}
		});

		yield return StartCoroutine(DelayPrimitive(3.0f));

		yield return SimplePrimitive(() =>
		{
			for (var i = 0; i < attachingObj.transform.childCount; i++)
			{
				var child = attachingObj.transform.GetChild(i).gameObject;
				var objectMeta = child.GetComponent<ObjectMeta>();

				if (objectMeta.status == ObjectMeta.Status.Dettached)
				{
					continue;
				}

				StartCoroutine(Instance.MakeObjectTransparent(child));
				StartCoroutine(SimplePrimitive(() => { objectMeta.status = ObjectMeta.Status.Dettached; }));
			}
		});

		yield return StartCoroutine(DelayPrimitive(0.5f));
		yield return null;
	}

	public IEnumerator GetAttachPrimitivesForParent(GameObject attachingObj, GameObject referenceObj)
	{
		var objectMeta = attachingObj.GetComponent<ObjectMeta>();

		if (objectMeta.status == ObjectMeta.Status.Attached)
		{
			yield return DelayPrimitive(1.0f);
			yield break;
		}

		yield return SimplePrimitive(() => { attachingObj.GetComponent<MeshRenderer>().enabled = true; });

		var text = "";
		const string delimiter = " and ";

		text += objectMeta.attachType switch
		{
			ObjectMeta.AttachTypes.SmoothInstall => "Smooth Install ",
			ObjectMeta.AttachTypes.StepInstall => "Step Install ",
			ObjectMeta.AttachTypes.SmoothScrew => "Smooth Screw ",
			ObjectMeta.AttachTypes.StepScrew => "Step Screw ",
			_ => "Smooth Install "
		};

		text += attachingObj.name + delimiter + referenceObj.name;
		yield return SimplePrimitive(() => { UIManager.UpdateReply(text); });

		yield return CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(attachingObj);
		yield return DelayPrimitive(0.5f);

		yield return Instance.ChangeObjectMaterialToInProgressCoroutine(attachingObj);
		yield return DelayPrimitive(2f);

		yield return Instance.CreateRotatePrimitives(attachingObj);
		yield return DelayPrimitive(1.0f);
		yield return Instance.CreateFromScatteredToRfmPrimitives(attachingObj, referenceObj);
		yield return DelayPrimitive(1.0f);
		yield return CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(attachingObj);

		var rotationAxis = objectMeta.attachRotationAxis;

		var attachRotationVector = rotationAxis switch
		{
			ObjectMeta.RotationAxisEnum.X => Vector3.right,
			ObjectMeta.RotationAxisEnum.NegX => Vector3.left,
			ObjectMeta.RotationAxisEnum.Y => Vector3.up,
			ObjectMeta.RotationAxisEnum.NegY => Vector3.down,
			ObjectMeta.RotationAxisEnum.Z => Vector3.forward,
			ObjectMeta.RotationAxisEnum.NegZ => Vector3.back,
			_ => Vector3.forward
		};

		switch (objectMeta.attachType)
		{
			case ObjectMeta.AttachTypes.SmoothInstall:
				yield return Instance.SmoothInstall(attachingObj, referenceObj, "attach");
				break;
			case ObjectMeta.AttachTypes.StepInstall:
				yield return Instance.StepInstall(attachingObj, referenceObj, "attach");
				break;
			case ObjectMeta.AttachTypes.SmoothScrew:
				yield return Instance.SmoothScrew(attachingObj, referenceObj, attachRotationVector, "attach");
				break;
			case ObjectMeta.AttachTypes.StepScrew:
				yield return Instance.StepScrew(attachingObj, referenceObj, attachRotationVector, "attach");
				break;
			default:
				yield return Instance.SmoothInstall(attachingObj, referenceObj, "attach");
				break;
		}

		yield return SimplePrimitive(() => { objectMeta.status = ObjectMeta.Status.Attached; });
		yield return DelayPrimitive(0.5f);
		yield return Instance.ResetObjectMaterial(attachingObj);
		yield return DelayPrimitive(2f);
	}

	public IEnumerator GetAttachPrimitivesForChildren(GameObject attachingObj, GameObject referenceObj)
	{
		yield return null;
	}
}