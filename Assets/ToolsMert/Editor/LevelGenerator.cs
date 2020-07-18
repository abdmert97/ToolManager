using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace ToolsMert.Editor
{
 
    public class LevelGenerator : EditorWindow
    {
        public static float posZ;
        public static Material mat;
        public static Vector3 size;

        public static GameObject source;
        public Color color;
        public int count;
        public Vector3 mousePos;
        public enum TYPE {Cube,Sphere,Capsule,Cylinder,Prefab};
        public static TYPE selectedType;
        public static bool perfectFit;
      
        private string _path;
        private string _loadPath;
        private GameObject _level;
        private int _matCount = 0;
        private List<string> _jsonString;


    
        [MenuItem("Window/LevelGenerator")]
        public static void ShowWindow()
        {
            GetWindow<LevelGenerator>("LevelGenerator");
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
        void OnGUI()
        {
     
            EditorGUILayout.BeginVertical();
            
            _path = EditorGUILayout.TextField("Asset saving path", _path);
            _loadPath= EditorGUILayout.TextField("Json Path", _loadPath);
            
            Space(2);
            GUILayout.Label("Level attributes", EditorStyles.boldLabel);
            
            source = (GameObject)EditorGUILayout.ObjectField("Prefab",source, typeof(GameObject), true);
            
            mat    = (Material)EditorGUILayout.ObjectField("Material", mat, typeof(Material), true);
            
            color  = EditorGUILayout.ColorField("Color", color);
            
            count  = EditorGUILayout.IntField("Count", count);
            
            size.x = EditorGUILayout.Slider("SizeX", size.x,1f,100f);
            size.y = EditorGUILayout.Slider("SizeY", size.y,1f,100f);
            size.z = EditorGUILayout.Slider("SizeZ", size.z,1f,100f);
            
            Space(2);
            perfectFit = EditorGUILayout.Toggle("Fit perfect", perfectFit);
            posZ = EditorGUILayout.FloatField("2D Z depth", posZ);
            
            Space(2);
            
            selectedType = (TYPE)EditorGUILayout.EnumPopup("Select Primitive Type", selectedType);
           
            Space(2);
            
            GUILayout.Label("Level Generator Buttons", EditorStyles.boldLabel);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginHorizontal();
            //   bool groupEnabled; groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled); Bunun altındakiler enabled disabled edilip açılabiliyor
        
            if (GUILayout.Button("Change Color"))
            {
                if(mat != null)
                    mat.color = color;       
            }
            if(GUILayout.Button("Clear Level"))
            {
                ClearLevel();
            }
            if(GUILayout.Button("Create Material"))
            {
                CreateMaterial();
            }
            EditorGUILayout.EndHorizontal();
            Space(3);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Level"))
            {
                LoadLevel();
            }
            if (GUILayout.Button("Save Level"))
            {
                SaveLevel();
            }
            EditorGUILayout.EndHorizontal();
            Space(3);
            GUILayout.Label("Action Type", EditorStyles.boldLabel);
            Handles.BeginGUI();
 
            //Here we draw a toolbar at the bottom edge of the SceneView
            EditorGUILayout.BeginHorizontal();
            {
                string[] buttonLabels = new string[] { "None", "Erase", "Paint" };

                ToolMenu.SelectedTool = GUILayout.SelectionGrid(
                    ToolMenu.SelectedTool,
                    buttonLabels,
                    3,
                    EditorStyles.toolbarButton,
                    GUILayout.Width(300));
            }
            EditorGUILayout.EndHorizontal();

            Handles.EndGUI();
        }
        void Space(int num)
        {
            for(int i = 0; i < num; i++)
                EditorGUILayout.Space();
        }
        void CreateObject(Vector3 pos,int count)
        {
            for(int i = 0; i < count; i++)
            {
                GameObject created = Instantiate(source, pos+Vector3.right*i, Quaternion.identity) as GameObject;
                created.GetComponent<Renderer>().sharedMaterial.color = color;
            }
       
        }
        void CreateMaterial()
        {
            Material material = new Material(Shader.Find("Specular"));
            string path2 = _path + _matCount.ToString()+".mat";
            AssetDatabase.CreateAsset(material,path2);
            _matCount++;
            mat = material;
 
        }
    
        void ClearLevel()
        {
            _level =GameObject.Find("Level");
            int count = _level.transform.childCount;
            for(int i = 1; i <= count; i++)
            {
                DestroyImmediate(_level.transform.GetChild(count -i).gameObject);
            }

        }
        void SaveLevel()
        {
            string path  = "";
            if(source!= null)
            {
                if (!AssetDatabase.Contains(source))
                {
                    Debug.LogError("Please use  prefabs to save the level");
                    return;
                }
                else
                {
                    path = AssetDatabase.GetAssetPath(source);
                    
                }
            }
            _level = GameObject.Find("Level");
            
            LevelSave levelText = new LevelSave();

            levelText.assetPath = path;

            int count = _level.transform.childCount;
            for (int i = 1; i <= count; i++)
            {
                Transform childTransform = _level.transform.GetChild(count - i).transform;
                Vector3 pos = childTransform.localPosition;
                Vector3 scale = childTransform.localScale;
                Vector3 rotation = childTransform.rotation.eulerAngles;
                levelText.posX.Add(pos.x);
                levelText.posY.Add(pos.y);
                levelText.posZ.Add(pos.z);
                
                levelText.scaleX.Add(scale.x);
                levelText.scaleY.Add(scale.y);
                levelText.scaleZ.Add(scale.z);

                levelText.rotationX.Add(rotation.x);
                levelText.rotationY.Add(rotation.y);
                levelText.rotationZ.Add(rotation.z);

            }
            SaveGame.saveGame(levelText);
        }
        void LoadLevel()
        {
 
            SaveGame.loadGame();
        }
    }
}
#endif