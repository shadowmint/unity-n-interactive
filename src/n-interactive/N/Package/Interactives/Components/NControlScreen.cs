using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace N.Package.Interactives.Components
{
    public class NControlScreen : MonoBehaviour
    {
        public GameObject a;
        public GameObject b;

        public Vector3[] points;

        private Vector3 _normal;

        public Vector3 up;
        public Vector3 right;

        public Color debugColor = Color.blue;

        public LinkedRefs linked;

        private Vector3 _pa;

        private Vector3 _pb;
        
        private Vector3 _height;
        private Vector3 _width;

        public Vector3 Center => points[0] + (points[2] - points[0]) / 2;

        public Vector3 Normal => linked.invertNormal ? -_normal : _normal;

        public Vector3 Height => _height;

        public Vector3 Width => _width;

        public void Update()
        {
            var didChange = false;

            var pa = a.transform.position;
            var pb = b.transform.position;

            if (Vector3.Distance(pa, _pa) > 0.05f)
            {
                didChange = true;
            }

            if (Vector3.Distance(pb, _pb) > 0.05f)
            {
                didChange = true;
            }

            if (didChange)
            {
                CalculateBoundingBox();
            }
        }

        public void CalculateBoundingBox()
        {
            if (a == null || b == null) return;

            _pa = a.transform.position;
            _pb = b.transform.position;

            var pa = _pa;
            var pb = _pb;

            if (points == null || points.Length != 4)
            {
                points = new Vector3[4];
            }

            var localUp = a.transform.up;

            var delta = pb - pa;
            _normal = Vector3.Cross(localUp, delta);

            var localRight = Vector3.Cross(_normal, localUp);

            _height = Vector3.Project(delta, localUp);
            up = localUp.normalized * _height.magnitude;
            
            _width = Vector3.Project(delta, localRight);
            
            points[0] = pa;
            points[1] = pa + _height;
            points[2] = pb;
            points[3] = pb - _height;

            if (linked.canvas != null)
            {
                linked.canvas.LinkedUpdate(this);
            }

            if (linked.surface != null)
            {
                linked.surface.LinkedUpdate(this);
            }
        }

        private void OnDrawGizmos()
        {
            if (a == null || b == null) return;
            CalculateBoundingBox();
            Gizmos.color = debugColor;
            Gizmos.DrawLine(points[0], points[1]);
            Gizmos.DrawLine(points[1], points[2]);
            Gizmos.DrawLine(points[2], points[3]);
            Gizmos.DrawLine(points[3], points[0]);
        }

        [System.Serializable]
        public struct LinkedRefs
        {
            public bool invertNormal;

            [Tooltip("Apply this screen to a linked world canvas by assigning it here")]
            public NControlScreenCanvas canvas;

            [Tooltip("Apply this screen to a linked world surface by assigning it here")]
            public NControlScreenSurface surface;
        }
    }
}