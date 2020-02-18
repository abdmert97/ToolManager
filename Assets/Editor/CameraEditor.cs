#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using Tools;
#if UNITY_EDITOR
namespace Editor
{
    [CustomEditor(typeof(CameraController))]
    public class CameraEditor : UnityEditor.Editor
    {
        public bool customSettings = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            customSettings = EditorGUILayout.Toggle("Custom settings", customSettings);
            if (customSettings)
            {
                CameraController cameraController = (CameraController) target;
                cameraController.relativeRotation = EditorGUILayout.Toggle("Relative rotation", cameraController.relativeRotation);
              
                cameraController.cullObjectFrontofTarget = EditorGUILayout.Toggle("Cull Object between camera and Target", cameraController.cullObjectFrontofTarget);
                if (cameraController.cullObjectFrontofTarget)
                {
                    cameraController.transparency =
                        EditorGUILayout.IntSlider("Transparency", cameraController.transparency,0,255);
                    cameraController.transparentMaterial = EditorGUILayout.ObjectField("Transparent Material",
                        cameraController.transparentMaterial, typeof(Material)) as Material;
                }  
                
                cameraController.fixedCamera = EditorGUILayout.Toggle("Fixed Camera", cameraController.fixedCamera);
                cameraController.autoCameraSpeed = EditorGUILayout.Toggle("Auto camera speed", cameraController.autoCameraSpeed);
                cameraController.stopCamera = EditorGUILayout.Toggle("Stop Camera", cameraController.stopCamera);
                cameraController.lookAlwaysTarget = EditorGUILayout.Toggle("Look always camera", cameraController.lookAlwaysTarget);
                cameraController.lerpAngle = EditorGUILayout.Toggle("Lerp Angle", cameraController.lerpAngle);
                
            }
            
        }
    }
}
#endif