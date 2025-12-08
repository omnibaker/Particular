using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sumfulla.Atomica
{
    public class OrbitRing : MonoBehaviour
    {
        [SerializeField] private float _lineWidth = .001f;
        [SerializeField] private Transform _orbiter;

        private LineRenderer _lr;

        private void Start()
        {
            _lr = GetComponent<LineRenderer>();
            DrawOrbit(_orbiter.localPosition.y);
        }

        /// <summary>
        /// Draws a ring at runtime for periodic energy shell
        /// </summary>
        private void DrawOrbit(float radius)
        {
            int segments = 360;
            _lr.useWorldSpace = false;
            _lr.startWidth = _lineWidth;
            _lr.endWidth = _lineWidth;
            _lr.positionCount = segments + 1;

            // Add extra point to make startpoint and endpoint the same to close the circle
            int pointCount = segments + 1;
            Vector3[] points = new Vector3[pointCount];

            // Loop points to create visible ring
            for (int i = 0; i < pointCount; i++)
            {
                float rad = Mathf.Deg2Rad * (i * 360f / segments);
                points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
            }

            // Add position coodinates to line renderer
            _lr.SetPositions(points);

            // Offset the display to create concentric rings
            transform.position += new Vector3(0, -radius, 0);
        }
    }
}