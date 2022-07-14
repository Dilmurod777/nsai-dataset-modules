using System.Collections;
using Custom;
using UnityEngine;

public class Robot : Singleton<Robot>
{
	private float _moveDuration;
	private float _rotateDuration;
	private float _defaultMoveDuration;
	private float _defaultRotateDuration;

	private void Start()
	{
		_moveDuration = 1.5f;
		_rotateDuration = 1.5f;
		_defaultMoveDuration = _moveDuration;
		_defaultRotateDuration = _rotateDuration;
	}

	public IEnumerator SetMoveDuration(float seconds)
	{
		_moveDuration = seconds;
		yield return null;
	}

	public IEnumerator SetRotateDuration(float seconds)
	{
		_rotateDuration = seconds;
		yield return null;
	}

	public float GetMoveDuration()
	{
		return _moveDuration;
	}

	public float GetRotateDuration()
	{
		return _rotateDuration;
	}

	public IEnumerator ResetMoveDuration()
	{
		_moveDuration = _defaultMoveDuration;
		yield return null;
	}

	public IEnumerator ResetRotateDuration()
	{
		_rotateDuration = _defaultRotateDuration;
		yield return null;
	}

	public IEnumerator Move(GameObject sourceObject, Vector3 finalPosition = default)
	{
		var initialPosition = sourceObject.transform.position;


		var delta = 0f;
		while (delta < GetMoveDuration())
		{
			delta += Time.fixedDeltaTime;
			// sourceObject.transform.position = Vector3.Lerp(initialPosition, ContextManager.Instance.latestGameObjectPositions[sourceObject.name], delta / GetMoveDuration());
			sourceObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, delta / GetMoveDuration());
			yield return null;
		}

		yield return null;
	}

	public IEnumerator Rotate(GameObject sourceObject, Vector3 finalRotation)
	{
		var initialRotation = sourceObject.transform.rotation;

		var delta = 0f;
		while (delta < GetRotateDuration())
		{
			delta += Time.fixedDeltaTime;
			sourceObject.transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(finalRotation), delta / GetRotateDuration());
			yield return null;
		}

		yield return null;
	}

	public IEnumerator MoveWithRotation(GameObject sourceObject, Vector3 finalPosition, Vector3 rotationAxis)
	{
		var initialPosition = sourceObject.transform.position;

		var delta = 0f;
		while (delta < GetMoveDuration())
		{
			delta += Time.fixedDeltaTime;
			sourceObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, delta / GetMoveDuration());
			sourceObject.transform.Rotate(rotationAxis, 2.5f);
			yield return null;
		}

		yield return null;
	}

	public IEnumerator Wait(float seconds = 0.0f)
	{
		yield return new WaitForSeconds(seconds);
	}
}