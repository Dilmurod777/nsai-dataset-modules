using System;
using System.Collections;
using Cinemachine;
using Custom;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
	private GameObject _virtualCamera;
	private GameObject _figureFocusVirtualCamera;

	private void Start()
	{
		_virtualCamera = GameObject.FindWithTag("VirtualCamera");
		_figureFocusVirtualCamera = GameObject.FindWithTag("FigureFocusVirtualCamera");
	}

	public IEnumerator UpdateVirtualCameraTargetCoroutine(GameObject target)
	{
		var vcComponent = _virtualCamera.GetComponent<CinemachineVirtualCamera>();
		var ffvcComponent = _figureFocusVirtualCamera.GetComponent<CinemachineVirtualCamera>();

		vcComponent.Priority = 11;
		ffvcComponent.Priority = 10;
		vcComponent.m_LookAt = target.transform;
		vcComponent.m_Follow = target.transform;

		// var volume = MeshVolume.Calculate(target.GetComponent<MeshFilter>().mesh);
		var volume = MeshVolume.GetVolume(target);

		var task = ContextManager.Instance.CurrentTask;
		var taskType = ContextManager.GetTaskType(task);
		var scale = taskType == ContextManager.TaskType.Installation ? 1.5f : 1.2f;

		var minFov = 60 * scale;
		var newOffsetZ = -0.75f;

		if (volume > 0.000000001)
		{
			minFov = 15 * scale;
			newOffsetZ *= 1.15f;
		}

		if (volume > 0.0000001)
		{
			minFov = 20 * scale;
			newOffsetZ *= 1.2f;
		}

		if (volume > 0.00001)
		{
			minFov = 30 * scale;
			newOffsetZ *= 1.25f;
		}

		if (volume > 0.0001)
		{
			minFov = 40 * scale;
			newOffsetZ *= 1.3f;
		}

		if (volume > 0.001)
		{
			minFov = 50 * scale;
			newOffsetZ *= 1.35f;
		}

		if (volume > 0.01)
		{
			minFov = 60 * scale;
			newOffsetZ *= 1.5f;
		}

		if (volume > 0.1)
		{
			minFov = 80 * scale;
			newOffsetZ *= 1.5f;
		}

		Debug.Log(target.name + " | " + volume + " | " + minFov + " | " + newOffsetZ);

		_virtualCamera.GetComponent<CinemachineFollowZoom>().m_MinFOV = minFov;

		var oldOffset = vcComponent.GetComponent<CinemachineCameraOffset>().m_Offset;
		vcComponent.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(oldOffset.x, oldOffset.y, newOffsetZ);

		yield return null;
	}

	public IEnumerator GetCameraCloser(float finalFov = 25.0f)
	{
		var initialMinFov = _virtualCamera.GetComponent<CinemachineFollowZoom>().m_MinFOV;
		const float duration = 2.0f;

		var delta = 0.0f;
		while (delta < duration)
		{
			delta += Time.fixedDeltaTime;
			_virtualCamera.GetComponent<CinemachineFollowZoom>().m_MinFOV = Mathf.Lerp(initialMinFov, finalFov, delta / duration);
			yield return null;
		}

		yield return null;
	}

	public void FocusOnFigure(GameObject target)
	{
		_figureFocusVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = target.transform;
		_figureFocusVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = target.transform;

		_figureFocusVirtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11;
		_virtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 10;

		Invoke("ResetFocusOnFigure", 2.0f);
	}

	public void ResetFocusOnFigure()
	{
		_figureFocusVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = null;
		_figureFocusVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = null;
	}
}