using System;
using System.Linq;
using N.Package.Interactives.Infrastructure;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace N.Package.Interactives.Components
{
    [RequireComponent(typeof(Collider))]
    public class NControlPlanarRotate : MonoBehaviour
    {
        public State state;
        public Refs refs;
        private Collider _collider;

        public void Awake()
        {
            if (refs.pointer == null)
            {
                refs.pointer = GetComponent<NControlPointer>();
            }

            if (refs.context == null)
            {
                refs.context = FindObjectOfType<NControlContext>();
            }

            _collider = GetComponent<Collider>();
            state.disabled = true;  // must explicitly activate first update
        }

        public void Update()
        {
            var disabled = refs.pointer == null || refs.context == null;
            if (disabled != state.disabled)
            {
                if (disabled)
                {
                    refs.pointer.RemoveEventHandler(OnStarted, OnCancelled);
                }
                else
                {
                    refs.pointer.OnTrigger(OnStarted, OnCancelled);
                }

                state.disabled = disabled;
            }
        }

        public void LateUpdate()
        {
            if (!state.dragging) return;
            state.currentPoint = PointerToWorld(refs.pointer, _collider, state.initialPoint);
            if (refs.applyTo?.Length > 0)
            {
                var ownTransform = transform;
                var rotationPlane = new Plane(state.originalAxis, ownTransform.position);
                for (var targetIndex = 0; targetIndex < refs.applyTo.Length; targetIndex++)
                {
                    var target = refs.applyTo[targetIndex];
                    var originalRotation = state.originalRotations[targetIndex];
                    var tp = target.transform.position;
                    var pointOnRotationPlane = rotationPlane.ClosestPointOnPlane(tp);
                    var normal = pointOnRotationPlane - tp;

                    var dl = state.initialPoint - pointOnRotationPlane;
                    var dc = state.currentPoint - pointOnRotationPlane;
                    var angle = Vector3.SignedAngle(dl, dc, state.originalAxis);
                    if (Mathf.Abs(angle) > 0)
                    {
                        // Depending on the orientation, we may want to invert the angle to match the normals.
                        angle = state.invertAngle ? -angle : angle;
                        target.transform.rotation = originalRotation;
                        target.transform.RotateAround(tp, state.originalAxis, angle);
                    }
                }
            }
        }

        private Vector3 PointerToWorld(NControlPointer pointer, Collider target, Vector3 initialPoint)
        {
            return refs.context.ProjectToPlane(refs.pointer, _collider, state.initialPoint);
        }

        public void StartMove()
        {
            OnStarted(default);
        }

        public void StopMove()
        {
            OnCancelled(default);
        }

        private void OnStarted(InputAction.CallbackContext context)
        {
            if (state.disabled) return;
            if (!refs.context.IsInteracting(refs.pointer, _collider, out var atPoint)) return;
            state.initialPoint = atPoint;
            state.dragging = true;
            state.originalRotations = refs.applyTo.Select(i => i.transform.rotation).ToArray();
            state.originalAxis = transform.forward;
        }

        private void OnCancelled(InputAction.CallbackContext context)
        {
            if (state.disabled) return;
            state.initialPoint = default;
            state.currentPoint = default;
            state.dragging = false;
            state.originalRotations = null;
        }


        [System.Serializable]
        public class State
        {
            public bool disabled;
            public bool dragging;
            public bool invertAngle;
            public Vector3 initialPoint;
            public Vector3 currentPoint;
            public Quaternion[] originalRotations;
            public Vector3 originalAxis;
            public Vector3 originalNormal; // TODO: this
        }

        [System.Serializable]
        public class Refs
        {
            public NControlPointer pointer;
            public NControlContext context;
            public GameObject[] applyTo;
        }
    }
}