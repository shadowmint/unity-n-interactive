using System;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

namespace N.Package.Interactives.Components
{
    public class NControlVolume : MonoBehaviour
    {
        public GameObject[] edges;

        public Bounds bounds;

        public Color debugColor = Color.blue;

        public void CalculateBoundingBox()
        {
            var min = edges.Length > 0 ? edges[0].transform.position : Vector3.zero;
            var max = edges.Length > 0 ? edges[0].transform.position : Vector3.zero;

            foreach (var edge in edges)
            {
                var p = edge.transform.position;
                if (p.x > max.x)
                {
                    max.x = p.x;
                }

                if (p.y > max.y)
                {
                    max.y = p.y;
                }

                if (p.z > max.z)
                {
                    max.z = p.z;
                }

                if (p.x < min.x)
                {
                    min.x = p.x;
                }

                if (p.y < min.y)
                {
                    min.y = p.y;
                }

                if (p.z < min.z)
                {
                    min.z = p.z;
                }
            }

            var size = (max - min);
            if (size.magnitude > 0)
            {
                bounds.center = min + size * 0.5f;
                bounds.size = size;
            }
            else
            {
                bounds.center = transform.position;
                bounds.size = Vector3.one;
            }
        }

        private void OnDrawGizmos()
        {
            CalculateBoundingBox();
            Gizmos.color = debugColor;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}