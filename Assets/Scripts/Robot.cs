﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using Custom;
using UnityEngine;

public class Robot : Singleton<Robot>
{
    private float _moveDuration;
    private float _rotateDuration;
    private float _scaleDuration;
    private float _defaultMoveDuration;
    private float _defaultRotateDuration;
    private float _defaultScaleDuration;

    private void Start()
    {
        _moveDuration = 1f;
        _rotateDuration = 1f;
        _scaleDuration = 1f;
        _defaultMoveDuration = _moveDuration;
        _defaultRotateDuration = _rotateDuration;
        _defaultScaleDuration = _scaleDuration;
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

        UIManager.Instance.UpdateBasicOperationsList("Move " + sourceObject.name + " " + initialPosition + ", " + finalPosition);

        var delta = 0f;
        while (delta < GetMoveDuration())
        {
            delta += Time.fixedDeltaTime;
            sourceObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, delta / GetMoveDuration());
            yield return null;
        }

        Helpers.WriteToLogs(new Dictionary<string, object>
        {
            {"Primitive", "Move"},
            {"Object name", sourceObject.name},
            {"Initial position", initialPosition},
            {"Theoretical final position", finalPosition},
            {"Practical final position", sourceObject.transform.position},
        });

        yield return null;
    }

    public IEnumerator Rotate(GameObject sourceObject, Vector3 finalRotation)
    {
        var initialRotation = sourceObject.transform.rotation;

        UIManager.Instance.UpdateBasicOperationsList("Rotate " + sourceObject.name + " " + initialRotation.eulerAngles + ", " + finalRotation);

        var delta = 0f;
        while (delta < GetRotateDuration())
        {
            delta += Time.fixedDeltaTime;
            sourceObject.transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(finalRotation), delta / GetRotateDuration());
            yield return null;
        }
        
        Helpers.WriteToLogs(new Dictionary<string, object>
        {
            {"Primitive", "Rotate"},
            {"Object name", sourceObject.name},
            {"Initial rotation", initialRotation},
            {"Theoretical final rotation", finalRotation},
            {"Practical final rotation", sourceObject.transform.rotation.eulerAngles},
        });

        yield return null;
    }

    public IEnumerator MoveWithRotation(GameObject sourceObject, Vector3 finalPosition, Vector3 rotationAxis)
    {
        var initialPosition = sourceObject.transform.position;
        var initialRotation = sourceObject.transform.rotation.eulerAngles;

        UIManager.Instance.UpdateBasicOperationsList("MoveWithRotation " + sourceObject.name + " " + initialPosition + ", " + finalPosition);

        var delta = 0f;
        while (delta < GetMoveDuration())
        {
            delta += Time.fixedDeltaTime;
            sourceObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, delta / GetMoveDuration());
            sourceObject.transform.Rotate(rotationAxis, 2.5f);
            yield return null;
        }
        
        Helpers.WriteToLogs(new Dictionary<string, object>
        {
            {"Primitive", "MoveWithRotation"},
            {"Object name", sourceObject.name},
            {"Initial position", initialPosition},
            {"Initial rotation", initialRotation},
            {"Rotation axis", rotationAxis},
            {"Theoretical final position", finalPosition},
            {"Practical final position", sourceObject.transform.position},
            {"Practical final rotation", sourceObject.transform.rotation.eulerAngles},
        });

        yield return null;
    }

    public IEnumerator Scale(GameObject sourceObject, Vector3 finalScale = default)
    {
        var initialScale = sourceObject.transform.localScale;

        UIManager.Instance.UpdateBasicOperationsList("Scale " + sourceObject.name + " " + initialScale + ", " + finalScale);

        var delta = 0f;
        while (delta < GetMoveDuration())
        {
            delta += Time.fixedDeltaTime;
            sourceObject.transform.localScale = Vector3.Lerp(initialScale, finalScale, delta / GetMoveDuration());
            yield return null;
        }

        yield return null;
    }

    public IEnumerator Wait(float seconds = 0.0f)
    {
        UIManager.Instance.UpdateBasicOperationsList("Wait " + seconds);

        yield return new WaitForSeconds(seconds);
    }
}