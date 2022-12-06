using Custom;
using UnityEngine;

public class GridManager
{
    private static readonly Vector3 _gridSize = new Vector3(0.5f, 0.5f, 0.5f);
    private static int _index;

    public static void SnapToGrid(GameObject obj)
    {
        var position = new Vector3(
            Mathf.RoundToInt(_index / _gridSize.x) * _gridSize.x,
            Mathf.RoundToInt(0 / _gridSize.y) * _gridSize.y,
            Mathf.RoundToInt(0 / _gridSize.z) * _gridSize.z
        );

        obj.transform.position = position;
        _index += Mathf.RoundToInt(_gridSize.x);
    }
}