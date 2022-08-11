using Custom;
using Instances;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
    public Material inProgressMaterial;
    public Material tempMaterial;

    public void UpdateAssets()
    {
        var task = ContextManager.Instance.CurrentTask;
        var subtask = ContextManager.Instance.CurrentSubtask;

        if (task != null && subtask != null)
        {
            var plainFigureName = Helpers.GetCurrentFigurePlainName();

            var figurePrefabInstallation =
                Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Reference);
            var figurePrefabRemoval =
                Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Ifm);

            var ifmPrefab = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Ifm);
            var rfmPrefab = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Rfm);
            var referencePrefab =
                Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Reference);

            if (referencePrefab != null)
            {
                var instantiatedReference = Instantiate(referencePrefab);
                instantiatedReference.tag = Tags.ReferenceObject;
                instantiatedReference.name = plainFigureName + Constants.FigureType.Reference;
                instantiatedReference.transform.position = Vector3.zero;
                instantiatedReference.transform.rotation = Quaternion.identity;
                instantiatedReference.AddComponent<CustomDontDestroyOnLoad>();

                foreach (var meshRenderer in instantiatedReference.GetComponentsInChildren<MeshRenderer>())
                    meshRenderer.enabled = false;
            }

            if (figurePrefabInstallation != null)
            {
                var instantiatedFigure = Instantiate(figurePrefabInstallation);
                instantiatedFigure.tag = Tags.Figure;
                instantiatedFigure.name = plainFigureName + Constants.TaskType.Installation;
                instantiatedFigure.transform.rotation = Quaternion.identity;
                instantiatedFigure.transform.position = Vector3.zero;
                instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

                foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
                {
                    if (child.name == instantiatedFigure.name) continue;

                    child.tag = Tags.Object;
                    var objectMeta = child.GetComponent<ObjectMeta>();

                    if (objectMeta == null) continue;
                    if (objectMeta.status == ObjectMeta.Status.Attached || objectMeta.isCoreInFigure) continue;

                    Destroy(child.gameObject);
                }
            }

            if (figurePrefabRemoval != null)
            {
                var instantiatedFigure = Instantiate(figurePrefabRemoval);
                instantiatedFigure.tag = Tags.Figure;
                instantiatedFigure.name = plainFigureName + Constants.TaskType.Removal;
                instantiatedFigure.transform.rotation = Quaternion.identity;
                instantiatedFigure.transform.position = Vector3.zero;
                instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

                foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
                {
                    if (child.name == instantiatedFigure.name) continue;

                    child.tag = Tags.Object;
                    var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
                    if (meshRenderer != null) meshRenderer.enabled = false;


                    var referenceChild = Helpers.FindObjectInFigure(Constants.FigureType.Reference, child.name);

                    if (referenceChild != null)
                    {
                        var childObjectMeta = child.GetComponent<ObjectMeta>();
                        if (childObjectMeta == null)
                        {
                            var referenceObjectMeta = referenceChild.GetComponent<ObjectMeta>();
                            if (referenceObjectMeta != null)
                            {
                                childObjectMeta = child.gameObject.AddComponent<ObjectMeta>();
                                childObjectMeta.attachType = referenceObjectMeta.attachType;
                                childObjectMeta.detachType = referenceObjectMeta.detachType;
                                childObjectMeta.attachRotationAxis = referenceObjectMeta.attachRotationAxis;
                                childObjectMeta.dettachRotationAxis = referenceObjectMeta.dettachRotationAxis;
                                childObjectMeta.status = referenceObjectMeta.status;
                                childObjectMeta.isCoreInFigure = referenceObjectMeta.isCoreInFigure;
                            }
                        }

                        var boxCollider = child.GetComponent<BoxCollider>();
                        if (boxCollider == null)
                        {
                            var referenceBoxCollider = referenceChild.GetComponent<BoxCollider>();

                            if (referenceBoxCollider != null)
                            {
                                boxCollider = child.gameObject.AddComponent<BoxCollider>();
                                boxCollider.isTrigger = referenceBoxCollider.isTrigger;
                                boxCollider.center = referenceBoxCollider.center;
                                boxCollider.size = referenceBoxCollider.size;
                            }
                        }
                    }
                }
            }

            if (ifmPrefab != null)
            {
                var instantiatedIfm = Instantiate(ifmPrefab);
                instantiatedIfm.tag = Tags.ReferenceObject;
                instantiatedIfm.name = plainFigureName + Constants.FigureType.Ifm;
                instantiatedIfm.transform.rotation = Quaternion.identity;
                instantiatedIfm.transform.position = Vector3.zero;
                instantiatedIfm.AddComponent<CustomDontDestroyOnLoad>();

                foreach (var meshRenderer in instantiatedIfm.GetComponentsInChildren<MeshRenderer>())
                    meshRenderer.enabled = false;
            }

            if (rfmPrefab != null)
            {
                var instantiatedRfm = Instantiate(rfmPrefab);
                instantiatedRfm.tag = Tags.ReferenceObject;
                instantiatedRfm.name = plainFigureName + Constants.FigureType.Rfm;
                instantiatedRfm.transform.rotation = Quaternion.identity;
                instantiatedRfm.transform.position = Vector3.zero;
                instantiatedRfm.AddComponent<CustomDontDestroyOnLoad>();

                foreach (var meshRenderer in instantiatedRfm.GetComponentsInChildren<MeshRenderer>())
                    meshRenderer.enabled = false;
            }
        }
    }

    public void ResetFigures()
    {
        var figures = GameObject.FindGameObjectsWithTag(Tags.Figure);

        foreach (var figure in figures)
        {
            ResetFigure(figure);
        }
    }

    public void ResetFigure(GameObject figure)
    {
        var plainFigureName = Helpers.GetCurrentFigurePlainName();
        var transformReferenceFigure = GameObject.Find(plainFigureName + Constants.FigureType.Reference)?.transform;

        if (transformReferenceFigure == null) return;

        figure.transform.position = Vector3.zero;
        figure.transform.rotation = Quaternion.identity;
        figure.transform.localScale = transformReferenceFigure.localScale;

        var infiniteRotation = figure.GetComponent<InfiniteRotation>();
        if (infiniteRotation)
        {
            infiniteRotation.enabled = false;
        }

        foreach (var child in figure.GetComponentsInChildren<Transform>())
        {
            if (child.name == figure.name) continue;

            var objectMeta = child.GetComponent<ObjectMeta>();
            if (objectMeta && objectMeta.isCoreInFigure) continue;

            var outline = child.GetComponent<Outline>();
            if (outline != null) Destroy(outline);


            var referenceChild = Helpers.FindObjectInFigure(Constants.FigureType.Reference, child.name);
            var transformReference = Helpers.FindObjectInFigure(Constants.FigureType.Ifm, child.name).transform;

            child.position = transformReference.position;
            child.rotation = transformReference.rotation;
            child.localScale = transformReference.localScale;

            if (referenceChild != null)
            {
                var meshRenderer = referenceChild.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    var materials = meshRenderer.materials;
                    child.GetComponent<MeshRenderer>().materials = materials;
                }

                if (objectMeta)
                {
                    var referenceObjectMeta = referenceChild.GetComponent<ObjectMeta>();

                    objectMeta.attachType = referenceObjectMeta.attachType;
                    objectMeta.detachType = referenceObjectMeta.detachType;
                    objectMeta.attachRotationAxis = referenceObjectMeta.attachRotationAxis;
                    objectMeta.dettachRotationAxis = referenceObjectMeta.dettachRotationAxis;
                    objectMeta.status = referenceObjectMeta.status;
                    objectMeta.isCoreInFigure = referenceObjectMeta.isCoreInFigure;

                    if (!figure.name.Contains(Constants.TaskType.Installation.ToString())) continue;
                    if (objectMeta.isCoreInFigure || objectMeta.status == ObjectMeta.Status.Attached) continue;
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public void ShowCurrentFigure()
    {
        var currentTask = ContextManager.Instance.CurrentTask;
        var taskType = ContextManager.GetTaskType(currentTask);
        var plainFigureName = Helpers.GetCurrentFigurePlainName();

        var figuresInScene = GameObject.FindGameObjectsWithTag(Tags.Figure);
        foreach (var figureInScene in figuresInScene)
        {
            if (taskType == Constants.TaskType.Installation)
                if (figureInScene.name == plainFigureName + Constants.TaskType.Installation)
                {
                    for (var i = 0; i < figureInScene.transform.childCount; i++)
                    {
                        var objectMeta = figureInScene.transform.GetChild(i).gameObject.GetComponent<ObjectMeta>();

                        if (objectMeta != null && objectMeta.isCoreInFigure)
                            CameraManager.Instance.FocusOnFigure(figureInScene.transform.GetChild(i).gameObject);

                        var meshRenderer = figureInScene.transform.GetChild(i).GetComponent<MeshRenderer>();
                        if (meshRenderer != null)
                            meshRenderer.enabled = objectMeta.status == ObjectMeta.Status.Attached;
                    }

                    break;
                }

            if (taskType == Constants.TaskType.Removal)
                if (figureInScene.name == plainFigureName + Constants.TaskType.Removal)
                {
                    foreach (var child in figureInScene.GetComponentsInChildren<Transform>())
                    {
                        var objectMeta = child.gameObject.GetComponent<ObjectMeta>();

                        if (objectMeta && objectMeta.isCoreInFigure)
                            CameraManager.Instance.FocusOnFigure(child.gameObject);

                        var meshRenderer = child.GetComponent<MeshRenderer>();
                        if (meshRenderer) meshRenderer.enabled = true;
                    }

                    break;
                }
        }

        ContextManager.Instance.ExecutePreviousInstructions(ContextManager.Instance.CurrentInstruction);
    }

    public void HideAllFigures()
    {
        var figuresInScene = GameObject.FindGameObjectsWithTag(Tags.Figure);
        foreach (var figureInScene in figuresInScene)
        foreach (var meshRenderer in figureInScene.GetComponentsInChildren<MeshRenderer>())
            meshRenderer.enabled = false;
    }

    public GameObject CreateCloneObject(GameObject obj, GameObject parent, string cloneTag = Tags.CloneObject, bool isEnabled = true,
        bool isPositionInherited = false, bool isRotationInherited = true, bool isObjectMetaInherited = false, bool isColliderInherited = false)
    {
        const float scale = 0.15f;
        var position = new Vector3(0, 0, parent.transform.childCount * scale);
        var rotation = Quaternion.identity;

        if (isPositionInherited) position = obj.transform.position;
        if (isRotationInherited) rotation = obj.transform.rotation;

        var cloneObject = Instantiate(obj, parent.transform);
        cloneObject.tag = cloneTag;
        cloneObject.transform.rotation = rotation;
        cloneObject.transform.position = position;

        var meshRenderer = cloneObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) return cloneObject;
        cloneObject.GetComponent<MeshRenderer>().enabled = isEnabled;

        var referenceObj = Helpers.FindObjectInFigure(Constants.FigureType.Reference, obj.name);

        if (isObjectMetaInherited)
        {
            var objectMeta = cloneObject.AddComponent<ObjectMeta>();
            var referenceObjectMeta = referenceObj.GetComponent<ObjectMeta>();

            if (referenceObjectMeta)
            {
                objectMeta.status = referenceObjectMeta.status;
                objectMeta.attachType = referenceObjectMeta.attachType;
                objectMeta.detachType = referenceObjectMeta.detachType;
                objectMeta.attachRotationAxis = referenceObjectMeta.attachRotationAxis;
                objectMeta.dettachRotationAxis = referenceObjectMeta.dettachRotationAxis;
                objectMeta.isCoreInFigure = referenceObjectMeta.isCoreInFigure;
            }
        }

        if (isColliderInherited)
        {
            var boxCollider = cloneObject.AddComponent<BoxCollider>();
            var referenceBoxCollider = referenceObj.GetComponent<BoxCollider>();

            if (referenceBoxCollider)
            {
                boxCollider.center = referenceBoxCollider.center;
                boxCollider.size = referenceBoxCollider.size;
                boxCollider.isTrigger = referenceBoxCollider.isTrigger;
            }
        }

        return cloneObject;
    }
}