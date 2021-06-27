using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace N.Package.Interactives.Components
{
    [RequireComponent(typeof(Collider))]
    public class NControlPlanarMove : MonoBehaviour
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
            state.lastPoint = state.currentPoint;
            state.currentPoint = PointerToWorld(refs.pointer, _collider, state.initialPoint);
            if (refs.applyTo?.Length > 0)
            {
                var delta = state.currentPoint - state.lastPoint;
                foreach (var target in refs.applyTo)
                {
                    target.transform.position += delta;
                }
            }
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
            state.currentPoint = PointerToWorld(refs.pointer, _collider, state.initialPoint);
            state.dragging = true;
        }

        private void OnCancelled(InputAction.CallbackContext context)
        {
            if (state.disabled) return;
            state.initialPoint = default;
            state.lastPoint = default;
            state.currentPoint = default;
            state.dragging = false;
        }

        private Vector3 PointerToWorld(NControlPointer pointer, Collider target, Vector3 initialPoint)
        {
            return state.restrictAxis
                ? refs.context.ProjectToAxis(refs.pointer, _collider, state.initialPoint)
                : refs.context.ProjectToPlane(refs.pointer, _collider, state.initialPoint);
        }

        [System.Serializable]
        public class State
        {
            [Tooltip("Set this to true to drag only in the primary (forward) axis direction")]
            public bool restrictAxis;

            public bool disabled;
            public bool dragging;
            public Vector3 initialPoint;
            public Vector3 currentPoint;
            public Vector3 lastPoint;
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