using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    [CustomEditor(typeof(Canvas))]
    public class CanvasEditor : UnityEditor.Editor
    {
       
        
        public void SetScreenSize()
        {
            Canvas canvas = (Canvas) target;
            var canvasScaler = canvas.gameObject.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920,1080);
            canvasScaler.matchWidthOrHeight = 1;
            canvasScaler.referencePixelsPerUnit = 128;
        }



        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            if (GUILayout.Button("Set Default Mobile Settings"))
            {
                SetScreenSize();
            }
            GUILayout.EndVertical();
        }
    }
}
