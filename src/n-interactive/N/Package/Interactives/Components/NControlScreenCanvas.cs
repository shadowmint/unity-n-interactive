using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace N.Package.Interactives.Components
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class NControlScreenCanvas : MonoBehaviour
    {
        private Canvas _canvas;

        private CanvasScaler _canvasScale;

        private Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponent<Canvas>();
                }

                return _canvas;
            }
        }

        private CanvasScaler Scale
        {
            get
            {
                if (_canvasScale == null)
                {
                    _canvasScale = GetComponent<CanvasScaler>();
                }

                return _canvasScale;
            }
        }

        public void LinkedUpdate(NControlScreen screen)
        {
            var rect = Canvas.transform as RectTransform;
            if (rect == null) return;

            rect.position = screen.Center;
            rect.rotation = Quaternion.LookRotation(screen.Normal, screen.up);

            var up = screen.up.magnitude;
            var right = screen.right.magnitude;

            var scale = Scale.scaleFactor;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            var localScale = rect.localScale;
            rect.sizeDelta = new Vector2(right / localScale.x, up / localScale.y);
        }
    }
}