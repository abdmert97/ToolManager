using UnityEditor;
using UnityEngine;

#if  UNITY_EDITOR

namespace Tools
{
    public class ToolWindow : EditorWindow
    {
  
        string tagName = "";
        bool groupEnabled;
        Prefabs prefabList;
        private int arrayCount;
        private GameObject arrayObject;
        private Vector3 arrayDistance;
        [MenuItem("Window/ToolWindow")]
        static void Init()
        { 
        
      
            // Get existing open window or if none, make a new one:
            ToolWindow window = (ToolWindow)EditorWindow.GetWindow(typeof(ToolWindow));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Space(5);
            prefabList = (Prefabs) EditorGUILayout.ObjectField("Prefab List", prefabList,typeof(Prefabs),false,GUILayout.Width(350)); 

            GUILayout.Space(10);
            if (GUILayout.Button("Create ToolManager",GUILayout.Width(350), GUILayout.Height(20)))
            {
                CreateToolManager();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Create Canvas",GUILayout.Width(350), GUILayout.Height(20)))
            {
                CreateCanvas();
            
            }
        
            GUILayout.Space(10);
            GUILayout.Label("GameObject Array Creator", EditorStyles.boldLabel);
            arrayObject = (GameObject) EditorGUILayout.ObjectField("Array GameObject", arrayObject,typeof(GameObject),true,GUILayout.Width(350), GUILayout.Height(15));
            arrayCount = EditorGUILayout.IntField("Array count", arrayCount, GUILayout.Width(200), GUILayout.Height(15));
        
            arrayDistance = EditorGUILayout.Vector3Field("Distance", arrayDistance,GUILayout.Width(350), GUILayout.Height(40));
            if (GUILayout.Button("Create Object Array",GUILayout.Width(350), GUILayout.Height(20)))
            {
                CreateObjectArray();
           
            }
            if (GUILayout.Button("Create 2D Object Array",GUILayout.Width(350), GUILayout.Height(20)))
            {
                Create2DObjectArray();
           
            }
        
       

            GUILayout.Label("Tag Creator", EditorStyles.boldLabel);
            // myString = EditorGUILayout.TextField("Text Field", myString);
            tagName = EditorGUILayout.TextField("Tag Name", tagName,GUILayout.Width(350), GUILayout.Height(15));
            if (GUILayout.Button("Add Tag",GUILayout.Width(350), GUILayout.Height(20)))
            {
                AddTag(tagName);
            } 
            // groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            // myBool = EditorGUILayout.Toggle("Toggle", myBool);
            // myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            // EditorGUILayout.EndToggleGroup();
        }

        private void CreateObjectArray()
        {
            if (arrayCount <= 0) return;
            GameObject array = new GameObject(arrayObject.name + " Array");
            for (int i = 0; i < arrayCount; i++)
            {
                Instantiate(arrayObject, i * arrayDistance, Quaternion.identity,array.transform);
            }
        }
        private void Create2DObjectArray()
        {
            if (arrayCount <= 0) return;
            GameObject array = new GameObject(arrayObject.name + " Array");
            for (int i = 0; i < arrayCount; i++)
            {
                for (int j = 0; j < arrayCount; j++)
                {
                    Instantiate(arrayObject, i * arrayDistance.x*Vector3.right+j*arrayDistance.z *Vector3.forward, Quaternion.identity,array.transform);    
                }
            
            }
        
        }

        private void AddTag(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
 
        
            SerializedProperty layersProp = tagManager.FindProperty("layers");
        
        
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName)) { found = true; break; }
            }
 
            // if not found, add it
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tagName;
                Debug.Log("Tag is added");
            }
            else
            {
                Debug.LogError("Tag already exists");
            }
        
            tagName = "";
            tagManager.ApplyModifiedProperties();
        
        
        }

        private  void CreateCanvas()
        {
        
            Instantiate(prefabList.canvas);
        }

        private void CreateToolManager()
        {
            if(FindObjectOfType<ToolManager>())
            {
                Debug.LogError("You already have Tool Manager");
                return;
            }
        
            var toolManager = new GameObject("Tool Manager");
            toolManager.AddComponent<ToolManager>();
        }
    }
}

#endif