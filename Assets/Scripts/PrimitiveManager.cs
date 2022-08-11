using System.Collections;
using System.Collections.Generic;
using Custom;
using Instances;
using UnityEditor.VersionControl;
using UnityEngine;

public class PrimitiveManager : Singleton<PrimitiveManager>
{
    public static IEnumerator SimplePrimitive(Constants.FunctionDelegate callback)
    {
        callback();
        yield return null;
    }

    public static IEnumerator DelayPrimitive(float seconds, Constants.FunctionDelegate callback = null)
    {
        yield return new WaitForSeconds(seconds);

        if (callback == null) yield break;

        callback();
        yield return null;
    }

    public IEnumerator CreateRotatePrimitives(string nameA)
    {
        var attachingObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
        var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, nameA);
        var finalRotation = referenceObj.transform.rotation.eulerAngles;

        yield return StartCoroutine(Robot.Instance.SetRotateDuration(0.5f));
        yield return StartCoroutine(Robot.Instance.Rotate(attachingObj, finalRotation));
        yield return StartCoroutine(Robot.Instance.ResetRotateDuration());
    }

    public IEnumerator CreateFromScatteredToRfmPrimitives(string nameA, string nameB)
    {
        var attachingObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
        var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);

        var rfmReferenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, nameA);
        var rfmReferenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, nameB);

        var rfmDiff = rfmReferenceObjA.transform.position - rfmReferenceObjB.transform.position;
        var finalPosition = referenceObj.transform.position + rfmDiff;

        yield return StartCoroutine(Robot.Instance.Move(attachingObj, finalPosition));
    }

    public IEnumerator SmoothInstall(string nameA, string nameB, string type)
    {
        yield return StartCoroutine(SimplePrimitive(() =>
        {
            GameObject referenceObjA, referenceObjB;

            var objA = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
            var objB = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);

            if (type == "attach")
            {
                referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objA.name);
                referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objB.name);
            }
            else
            {
                referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objA.name);
                referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objB.name);
            }

            var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
            var finalPosition = objB.transform.position + rfmDiff;
            StartCoroutine(Robot.Instance.Move(objA, finalPosition));
        }));
    }

    public IEnumerator SmoothScrew(string nameA, string nameB, Vector3 direction, string type)
    {
        GameObject referenceObjA, referenceObjB;

        var objA = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
        var objB = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);

        if (type == "attach")
        {
            referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objA.name);
            referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objB.name);
        }
        else
        {
            referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objA.name);
            referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objB.name);
        }

        var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
        var finalPosition = objB.transform.position + rfmDiff;

        yield return StartCoroutine(Robot.Instance.MoveWithRotation(objA, finalPosition, direction));
    }

    public IEnumerator StepInstall(string nameA, string nameB, string type, int steps = 3)
    {
        GameObject referenceObjA, referenceObjB;

        var objA = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
        var objB = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);

        if (type == "attach")
        {
            referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objA.name);
            referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objB.name);
        }
        else
        {
            referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objA.name);
            referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objB.name);
        }

        var rfmDiff = referenceObjA.transform.position - referenceObjB.transform.position;
        var delta = objB.transform.position + rfmDiff - objA.transform.position;

        for (var i = 0; i < steps; i++)
        {
            yield return StartCoroutine(Robot.Instance.Move(objA, objA.transform.position + (i + 1) * delta / steps));
            yield return StartCoroutine(Robot.Instance.Wait(0.25f));
        }
    }

    public IEnumerator StepScrew(string nameA, string nameB, Vector3 direction, string type, int steps = 3)
    {
        GameObject referenceObjA, referenceObjB;

        var objA = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
        var objB = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);

        if (type == "attach")
        {
            referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objA.name);
            referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, objB.name);
        }
        else
        {
            referenceObjA = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objA.name);
            referenceObjB = Helpers.FindObjectInFigure(Constants.FigureType.Rfm, objB.name);
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

    public IEnumerator ChangeObjectMaterialToInProgressCoroutine(string objName)
    {
        var obj = Helpers.FindObjectInFigure(Constants.FigureType.Current, objName);
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

    public IEnumerator ResetObjectMaterial(string name)
    {
        var obj = Helpers.FindObjectInFigure(Constants.FigureType.Current, name);
        var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Reference, obj.name);

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

        yield return CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(attachingObj.name);
        yield return DelayPrimitive(0.5f);

        yield return Instance.ChangeObjectMaterialToInProgressCoroutine(attachingObj.name);
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
                yield return Instance.SmoothInstall(attachingObj.name, referenceObj.name, "detach");
                break;
            case ObjectMeta.AttachTypes.StepInstall:
                yield return Instance.StepInstall(attachingObj.name, referenceObj.name, "detach");
                break;
            case ObjectMeta.AttachTypes.SmoothScrew:
                yield return Instance.SmoothScrew(attachingObj.name, referenceObj.name, attachRotationVector, "detach");
                break;
            case ObjectMeta.AttachTypes.StepScrew:
                yield return Instance.StepScrew(attachingObj.name, referenceObj.name, attachRotationVector, "detach");
                break;
            default:
                yield return Instance.SmoothInstall(attachingObj.name, referenceObj.name, "detach");
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
                        StartCoroutine(Instance.SmoothInstall(child.name, referenceObj.name, "detach"));
                        break;
                    case ObjectMeta.AttachTypes.StepInstall:
                        StartCoroutine(Instance.StepInstall(child.name, referenceObj.name, "detach"));
                        break;
                    case ObjectMeta.AttachTypes.SmoothScrew:
                        StartCoroutine(Instance.SmoothScrew(child.name, referenceObj.name, attachRotationVector, "detach"));
                        break;
                    case ObjectMeta.AttachTypes.StepScrew:
                        StartCoroutine(Instance.StepScrew(child.name, referenceObj.name, attachRotationVector, "detach"));
                        break;
                    default:
                        StartCoroutine(Instance.SmoothInstall(child.name, referenceObj.name, "detach"));
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

    public IEnumerator GetAttachPrimitivesForParent(string nameA, string nameB)
    {
        yield return SimplePrimitive(() =>
        {
            var attachingObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
            attachingObj.GetComponent<MeshRenderer>().enabled = true;
        });


        yield return SimplePrimitive(() =>
        {
            var attachingObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameA);
            var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Current, nameB);
            var objectMeta = attachingObj.GetComponent<ObjectMeta>();

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
            UIManager.UpdateReply(text);
        });

        yield return CameraManager.Instance.UpdateVirtualCameraTargetCoroutine(nameA);
        yield return DelayPrimitive(0.5f);

        yield return Instance.ChangeObjectMaterialToInProgressCoroutine(nameA);
        yield return DelayPrimitive(2f);

        yield return Instance.CreateRotatePrimitives(nameA);
        yield return DelayPrimitive(1.0f);
        yield return Instance.CreateFromScatteredToRfmPrimitives(nameA, nameB);
        yield return DelayPrimitive(1.0f);

        var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Reference, nameA);
        var objectMeta = referenceObj.GetComponent<ObjectMeta>();
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
                yield return Instance.SmoothInstall(nameA, nameB, "attach");
                break;
            case ObjectMeta.AttachTypes.StepInstall:
                yield return Instance.StepInstall(nameA, nameB, "attach");
                break;
            case ObjectMeta.AttachTypes.SmoothScrew:
                yield return Instance.SmoothScrew(nameA, nameB, attachRotationVector, "attach");
                break;
            case ObjectMeta.AttachTypes.StepScrew:
                yield return Instance.StepScrew(nameA, nameB, attachRotationVector, "attach");
                break;
            default:
                yield return Instance.SmoothInstall(nameA, nameB, "attach");
                break;
        }

        yield return SimplePrimitive(() => { objectMeta.status = ObjectMeta.Status.Attached; });
        yield return DelayPrimitive(0.5f);
        yield return Instance.ResetObjectMaterial(nameA);
        yield return DelayPrimitive(2f);
    }

    public IEnumerator GetAttachPrimitivesForChildren(string nameA, string nameB)
    {
        yield return null;
    }
}