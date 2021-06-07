using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace N.Package.Interactives.Components
{
    public class NControlScreenSurface : MonoBehaviour
    {
        public bool invertNormal;

        public Vector3 unitScale = Vector3.one;

        public void LinkedUpdate(NControlScreen screen)
        {
            var trans = transform;

            trans.position = screen.Center;
            trans.rotation = Quaternion.LookRotation(invertNormal ? -screen.normal : screen.normal, screen.up);

            var up = screen.up.magnitude;
            var right = screen.right.magnitude;
            trans.localScale = new Vector3(right * unitScale.x, up * unitScale.y, 1);
        }
    }
}