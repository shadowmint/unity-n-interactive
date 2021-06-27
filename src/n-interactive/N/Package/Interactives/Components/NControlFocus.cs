using System;
using System.Collections.Generic;
using System.Linq;
using N.Package.Interactives.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace N.Package.Interactives.Components
{
    [RequireComponent(typeof(Collider))]
    public class NControlFocus : MonoBehaviour
    {
        public State state;
        public Refs refs;
        public NControlFocusEvent onChange;

        private readonly List<Collider> _colliders = new List<Collider>();

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

            _colliders.Add(GetComponent<Collider>());
            state.disabled = true; // must explicitly activate first update
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
                    LostFocus(Vector3.zero);
                }

                state.disabled = disabled;
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
            if (!state.focus)
            {
                if (refs.context.IsInteracting(refs.pointer, _colliders[0], out var atPoint))
                {
                    GainedFocus(atPoint);
                }
            }
        }

        private void OnCancelled(InputAction.CallbackContext context)
        {
            if (state.disabled) return;
            if (state.focus)
            {
                var atPoint = Vector3.zero;
                var anyFocus = _colliders.Any(trackingTarget => refs.context.IsInteracting(refs.pointer, trackingTarget, out atPoint));
                if (!anyFocus)
                {
                    LostFocus(atPoint);
                }
            }
        }

        private void GainedFocus(Vector3 atPoint)
        {
            state.focus = true;
            state.initialPoint = atPoint;
            foreach (var target in refs.withFocus)
            {
                var childCollider = target.GetComponent<Collider>();
                if (childCollider != null && !_colliders.Contains(childCollider))
                {
                    _colliders.Add(childCollider);
                }
            }

            onChange.Invoke(this);
        }

        private void LostFocus(Vector3 atPoint)
        {
            state.focus = false;
            state.lastPoint = atPoint;
            foreach (var target in refs.withFocus)
            {
                var childCollider = target.GetComponent<Collider>();
                if (childCollider != null && _colliders.Contains(childCollider))
                {
                    _colliders.Remove(childCollider);
                }
            }

            onChange.Invoke(this);
        }

        [System.Serializable]
        public class State
        {
            public bool focus;
            public bool disabled;
            public Vector3 initialPoint;
            public Vector3 lastPoint;
        }

        [System.Serializable]
        public class Refs
        {
            public NControlPointer pointer;
            public NControlContext context;

            [Tooltip("Keep focus when interacting with these objects")]
            public GameObject[] withFocus;
        }
    }
}