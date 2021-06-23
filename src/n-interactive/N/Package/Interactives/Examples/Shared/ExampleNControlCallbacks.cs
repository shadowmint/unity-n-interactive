using N.Package.Interactives.Components;
using UnityEngine;

namespace N.Package.Interactives.Examples.Shared
{
    public class ExampleNControlCallbacks : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public void OnFocusChanged(NControlFocus focus)
        {
            var urpRenderer = GetComponent<MeshRenderer>();
            var block = new MaterialPropertyBlock();
            block.SetColor(BaseColor, focus.state.focus ? Color.blue : Color.white);
            Debug.Log($"Changed Focus: {focus.state.focus}");
            urpRenderer.SetPropertyBlock(block);
        }
    }
}