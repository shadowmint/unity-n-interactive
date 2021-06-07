using System;
using System.Numerics;
using N.Package.Interactives.Infrastructure;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Vector3 = UnityEngine.Vector3;

namespace N.Package.Interactives.Components
{
    public class NControlContext : MonoBehaviour
    {
        public Config config;

        private NRaycastTools _raycast;

        private void Awake()
        {
            _raycast = new NRaycastTools(config.rayCastCapacity, config.rayCastMask);
        }

        [System.Serializable]
        public class Config
        {
            public Camera camera;

            [Range(1, 256)]
            public int rayCastCapacity = 16;

            public int rayCastMask = -1;
        }

        public bool IsInteracting(NControlPointer pointer, Collider target, out Vector3 atPoint)
        {
            var here = pointer.GetAxisValue();
            var from = _raycast.RaycastFrom(here, config.camera);
            return _raycast.Intersects(target, from, out atPoint);
        }

        public Vector3 ProjectToPlane(NControlPointer pointer, Collider target, Vector3 initialPoint)
        {
            var here = pointer.GetAxisValue();
            _raycast.RaycastFrom(here, config.camera);
            var plane = new Plane(target.transform.forward, initialPoint);
            return _raycast.RaycastToPlane(plane);
        }

        public Vector3 ProjectToAxis(NControlPointer pointer, Collider target, Vector3 initialPoint)
        {
            var here = pointer.GetAxisValue();
            _raycast.RaycastFrom(here, config.camera);
            var targetTrans = target.transform;
            var plane = new Plane(targetTrans.up, initialPoint);
            var pointOnPlane = _raycast.RaycastToPlane(plane);
            var pointOnAxis = Vector3.Project(pointOnPlane, targetTrans.forward);
            return pointOnAxis;
        }
    }
}