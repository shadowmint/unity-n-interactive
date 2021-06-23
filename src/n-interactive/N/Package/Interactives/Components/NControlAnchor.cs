using UnityEditorInternal;
using UnityEngine;

namespace N.Package.Interactives.Components
{
    /// <summary>
    /// Use this component to anchor a root object to a target.
    /// For example, you can use this to position a world-space UI screen in front of a camera.
    /// </summary>
    [ExecuteAlways]
    public class NControlAnchor : MonoBehaviour
    {
        public GameObject anchor;
        public Vector3 offset;
        public Vector3 rotation;

        [Range(0.0001f, 1)]
        public float movementThreshold = 0.01f;

        [Range(0.0001f, 1)]
        public float rotationThreshold = 0.1f;

        private GameObject _anchor;
        private Vector3 _position;
        private Quaternion _rotation;

        public void Update()
        {
            if (anchor == null) return;

            if (_anchor != anchor)
            {
                InitializeState();
                return;
            }

            var anchorPosition = _anchor.transform.position;
            var anchorRotation = _anchor.transform.rotation;

            if (Vector3.Distance(anchorPosition, _position) > movementThreshold || Quaternion.Angle(anchorRotation, _rotation) > rotationThreshold)
            {
                UpdateState(anchorPosition);
            }
        }

        private void UpdateState(Vector3 anchorPosition)
        {
            var up = _anchor.transform.up;
            var forward = _anchor.transform.forward;
            var right = _anchor.transform.right;
            var relativeOffset = offset.y * Vector3.up + offset.x * right + offset.z * forward;

            var ownTransform = transform;
            var deltaToAnchor = anchorPosition - ownTransform.position;
            ownTransform.rotation = Quaternion.LookRotation(deltaToAnchor, up) * Quaternion.Euler(rotation);
            ownTransform.position = anchorPosition + relativeOffset;
        }

        private void InitializeState()
        {
            _anchor = anchor;

            _position = _anchor.transform.position;
            _rotation = _anchor.transform.rotation;

            offset = transform.position - _position;
            rotation = Vector3.zero;

            UpdateState(_position);
        }
    }
}