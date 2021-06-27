using N.Package.Interactives.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace N.Package.Interactives.Infrastructure
{
    public class NRaycastTools
    {
        public float MaxDistance { get; set; } = 100f;

        private int _lastHits;

        private readonly RaycastHit[] _hitBuffer;

        private readonly int _layerMask;

        private Ray _lastRay;

        private int _lastCast;

        public NRaycastTools(int capacity, int layerMask)
        {
            _hitBuffer = new RaycastHit[capacity];
            _layerMask = layerMask;
            _lastCast = 0;
        }

        public Vector3 RaycastFrom(Vector2 origin, Camera camera)
        {
            var thisFrame = Time.frameCount;
            if (_lastCast == thisFrame) return _lastRay.origin;
            camera = camera == null ? Camera.current : camera;
            _lastRay = camera.ScreenPointToRay(origin);
            _lastHits = Physics.RaycastNonAlloc(_lastRay, _hitBuffer, MaxDistance, _layerMask);
            _lastCast = thisFrame;
            return _lastRay.origin;
        }

        public static Vector3 RaycastToPlane(Plane plane, Ray ray)
        {
            return !plane.Raycast(ray, out var enter) ? Vector3.zero : ray.GetPoint(enter);
        }

        public Vector3 RaycastToPlane(Plane plane)
        {
            return RaycastToPlane(plane, _lastRay);
        }

        public bool Intersects(Collider collider, Vector3 fromPoint, out Vector3 atPoint)
        {
            var lowestDepth = -1f;
            var bestPoint = Vector3.zero;
            var isMatch = false;
            for (var i = 0; i < _lastHits; i++)
            {
                var p = _hitBuffer[i].point;
                var depth = _hitBuffer[i].distance;
                if (lowestDepth < 0 || lowestDepth > depth)
                {
                    bestPoint = p;
                    lowestDepth = depth;
                    isMatch = _hitBuffer[i].collider == collider;
                }
            }

            atPoint = bestPoint;
            return isMatch && lowestDepth >= 0f;
        }
    }
}