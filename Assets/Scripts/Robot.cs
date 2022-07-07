using System.Collections;
using UnityEngine;

public class Robot
{
    private float _moveDuration;
    private float _rotateDuration;

    private const float MoveThreshold = 1.5f;
    private const float RotateThreshold = 0.2f;

    public Robot(float moveDuration = 1.5f, float rotateDuration = 1.5f)
    {
        _moveDuration = moveDuration;
        _rotateDuration = rotateDuration;
    }

    public float GetMoveDuration()
    {
        return _moveDuration;
    }

    public float GetRotateDuration()
    {
        return _rotateDuration;
    }

    public IEnumerator Move(GameObject sourceObject, Vector3 finalPosition)
    {
        var initialPosition = sourceObject.transform.position;

        var delta = 0f;
        while (delta < GetMoveDuration())
        {
            delta += Time.deltaTime;
            sourceObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, delta);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator Rotate(GameObject sourceObject, Vector3 finalRotation)
    {
        var initialRotation = sourceObject.transform.rotation;

        var delta = 0f;
        while (delta < GetMoveDuration())
        {
            delta += Time.deltaTime;
            sourceObject.transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(finalRotation), delta);
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
            sourceObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, delta);
            sourceObject.transform.rotation = Quaternion.Lerp(sourceObject.transform.rotation, Quaternion.Euler(finalRotation), delta);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator Wait(float duration = 0.0f)
    {
        yield return new WaitForSeconds(duration);
    }
}