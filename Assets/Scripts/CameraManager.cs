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
		_virtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11;
		_figureFocusVirtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 10;
		_virtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = target.transform;
		_virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = target.transform;

		var volume = MeshVolume.Calculate(target.GetComponent<MeshFilter>().mesh);

		var task = ContextManager.Instance.CurrentTask;
		var taskType = ContextManager.GetTaskType(task);
		var scale = taskType == ContextManager.TaskType.Installation ? 2.5f : 1f;

		var minFov = 5 * scale;
		if (volume > 0.000000001)
		{
			minFov = 4 * scale;
		}

		if (volume > 0.0000001)
		{
			minFov = 8 * scale;
		}

		if (volume > 0.00001)
		{
			minFov = 12 * scale;
		}

		if (volume > 0.0001)
		{
			minFov = 16 * scale;
		}

		if (volume > 0.001)
		{
			minFov = 20 * scale;
		}

		Debug.Log(target.name + " | " + volume + " | " + minFov);

		_virtualCamera.GetComponent<CinemachineFollowZoom>().m_MinFOV = minFov;
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
	}
}