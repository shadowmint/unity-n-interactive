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

            if (refs.pointer == null)
            {
                Debug.LogWarning($"{transform} has no pointer assigned, applying auto-disable.");
                gameObject.SetActive(false);
                state.disabled = true;
            }

            if (refs.context == null)
            {
                refs.context = FindObjectOfType<NControlContext>();
            }

            if (refs.context == null)
            {
                Debug.LogWarning($"{transform} has no context assigned, applying auto-disable.");
                gameObject.SetActive(false);
                state.disabled = true;
            }

            if (!state.disabled)
            {
                refs.pointer.OnTrigger(OnStarted, OnCancelled);
            }

            _colliders.Add(GetComponent<Collider>());

            LostFocus(Vector3.zero);
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
                Debug.Log($"Had any focus: {anyFocus} from {_colliders.Count} targets");
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
                target.gameObject.SetActive(true);
                if (state.keepFocus)
                {
                    var childCollider = target.GetComponent<Collider>();
                    if (childCollider != null && !_colliders.Contains(childCollider))
                    {
                        _colliders.Add(childCollider);
                    }
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
                target.gameObject.SetActive(false);
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

            [Tooltip("If true, track the colliders of the 'withFocus' refs")]
            public bool keepFocus;
        }

        [System.Serializable]
        public class Refs
        {
            public NControlPointer pointer;
            public NControlContext context;
            public GameObject[] withFocus;
        }
    }
}