using System.Collections;
using UnityEngine;

public class Robot
{
	private float _moveDuration;
	private float _rotateDuration;
	private float _defaultMoveDuration;
	private float _defaultRotateDuration;

	public Robot(float moveDuration = 1.5f, float rotateDuration = 1.5f)
	{
		_moveDuration = moveDuration;
		_rotateDuration = rotateDuration;
		_defaultMoveDuration = moveDuration;
		_defaultRotateDuration = rotateDuration;
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

	public IEnumerator Move(GameObject sourceObject, Vector3 finalPosition)
	{
		var initialPosition = sourceObject.transform.position;

		var delta = 0f;
		while (delta < GetMoveDuration())
		{
			delta += Time.deltaTime;
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
			delta += Time.deltaTime;
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
			var finalRotation = sourceObject.transform.eulerAngles + rotationAxis * 5.0f;

			delta += Time.deltaTime;
			sourceObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, delta/GetMoveDuration());
			sourceObject.transform.rotation = Quaternion.Lerp(sourceObject.transform.rotation, Quaternion.Euler(finalRotation), delta/GetRotateDuration());
			yield return null;
		}

		yield return null;
	}

	public IEnumerator Wait(float seconds = 0.0f)
	{
		yield return new WaitForSeconds(seconds);
	}
}