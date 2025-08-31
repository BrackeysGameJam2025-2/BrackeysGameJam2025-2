using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    private Vector3[] _points;

    public IReadOnlyList<Vector3> Points => _points;

    private void Awake()
    {
        _points = GetPoints();
    }

    // A little bit of trolling
    private Vector3[] GetPoints() => (from i in Enumerable.Range(0, transform.childCount)
                                      select transform.GetChild(i).position).ToArray();

    public int FindNearestPointIndex(Vector3 position)
    {
        if (_points.Length == 0) return -1;

        float minDistanceSql = Vector3.SqrMagnitude(position - _points[0]);
        int minIndex = 0;
        for (int i = 1; i < _points.Length; i++)
        {
            float distanceSqr = Vector3.SqrMagnitude(position - _points[i]);
            if (distanceSqr < minDistanceSql) minIndex = i;
        }
        return minIndex;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3[] points = GetPoints();
        Gizmos.color = Color.blue;
        Gizmos.DrawLineStrip(points.AsSpan(), looped: true);

        Color startColor = Color.blue;
        Color endColor = Color.red;
        for (int i = 0; i < points.Length; i++)
        {
            float blend = points.Length == 1 ? 0f : (float)i / (points.Length - 1);
            Gizmos.color = Color.Lerp(startColor, endColor, blend);
            Gizmos.DrawWireSphere(points[i], 1f);
        }
    }
#endif
}
