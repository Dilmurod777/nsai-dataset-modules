using System;
using System.Collections;
using Cinemachine;
using Custom;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
	private GameObject _virtualCamera;

	private void Start()
	{
		_virtualCamera = GameObject.FindWithTag("VirtualCamera");
	}

	public IEnumerator UpdateVirtualCameraTarget(GameObject target)
	{
		_virtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = target.transform;
		_virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = target.transform;

		var volume = MeshVolume.Calculate(target.GetComponent<MeshFilter>().mesh);
		Debug.Log(target.name + " | " + volume);

		var minFov = 20;
		if (volume > 0.000000001)
		{
			minFov = 10;
		}

		if (volume > 0.0000001)
		{
			minFov = 15;
		}

		if (volume > 0.00001)
		{
			minFov = 25;
		}
		
		_virtualCamera.GetComponent<CinemachineFollowZoom>().m_MinFOV = minFov;
		yield return null;
	}
}