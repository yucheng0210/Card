using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Radishmouse
{
    public class UILineRenderer : Graphic
    {
        public Vector2[] points;

        public float thickness = 10f;
        public bool center = true;

        public float dashLength = 5f;  // 虛線的線段長度
        public float gapLength = 10f;   // 虛線的間隔長度

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (points.Length < 2)
                return;

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 start = points[i];
                Vector2 end = points[i + 1];

                float segmentLength = Vector2.Distance(start, end);
                Vector2 direction = (end - start).normalized;

                float currentLength = 0f;
                bool draw = true;
                List<Vector2> dashedPoints = new List<Vector2>();

                while (currentLength < segmentLength)
                {
                    float segment = draw ? dashLength : gapLength;

                    if (currentLength + segment > segmentLength)
                        segment = segmentLength - currentLength;

                    if (draw)
                    {
                        dashedPoints.Add(start + direction * currentLength);
                        dashedPoints.Add(start + direction * (currentLength + segment));
                    }

                    currentLength += segment;
                    draw = !draw;
                }

                for (int j = 0; j < dashedPoints.Count - 1; j += 2)
                {
                    CreateLineSegment(dashedPoints[j], dashedPoints[j + 1], vh);
                }
            }
        }

        /// <summary>
        /// Creates a rect from two points that acts as a line segment
        /// </summary>
        private void CreateLineSegment(Vector3 point1, Vector3 point2, VertexHelper vh)
        {
            Vector3 offset = center ? (rectTransform.sizeDelta / 2) : Vector2.zero;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            Quaternion rotation = Quaternion.Euler(0, 0, RotatePointTowards(point1, point2) + 90);
            vertex.position = rotation * new Vector3(-thickness / 2, 0) + point1 - offset;
            vh.AddVert(vertex);
            vertex.position = rotation * new Vector3(thickness / 2, 0) + point1 - offset;
            vh.AddVert(vertex);

            vertex.position = rotation * new Vector3(-thickness / 2, 0) + point2 - offset;
            vh.AddVert(vertex);
            vertex.position = rotation * new Vector3(thickness / 2, 0) + point2 - offset;
            vh.AddVert(vertex);

            int index = vh.currentVertCount - 4;
            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 1, index + 3);
        }

        /// <summary>
        /// Gets the angle that a vertex needs to rotate to face target vertex
        /// </summary>
        private float RotatePointTowards(Vector2 vertex, Vector2 target)
        {
            return Mathf.Atan2(target.y - vertex.y, target.x - vertex.x) * Mathf.Rad2Deg;
        }
    }
}
