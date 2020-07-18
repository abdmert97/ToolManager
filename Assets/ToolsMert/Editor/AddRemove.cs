using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
namespace ToolsMert.Editor
{
    [InitializeOnLoad]
    public class AddRemove : UnityEditor.Editor
    {
        static Transform m_LevelParent;
        public static Transform LevelParent
        {
            get
            {
                if (m_LevelParent == null)
                {
                    GameObject level = GameObject.Find("Level");

                    if (level != null)
                    {
                        m_LevelParent = level.transform;
                    }
                    else
                    {
                        level = new GameObject("Level");
                        m_LevelParent = level.transform;
                    }
                }

                return m_LevelParent;
            }
        }

        static AddRemove()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
       

            if (ToolMenu.SelectedTool == 0)
            {
                return;
            }

            //By creating a new ControlID here we can grab the mouse input to the SceneView and prevent Unitys default mouse handling from happening
            //FocusType.Passive means this control cannot receive keyboard input since we are only interested in mouse input
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            //If the left mouse is being clicked and no modifier buttons are being held
            if (Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                Event.current.alt == false &&
                Event.current.shift == false &&
                Event.current.control == false)
            {
                if (LevelEditor.IsMouseInValidArea == true)
                {
                    if (ToolMenu.SelectedTool == 1)
                    {
                        //If there eraser tool is selected, erase the block at the current block handle position
                        RemoveBlock(LevelEditor.CurrentHandlePosition);
                    }

                    if (ToolMenu.SelectedTool == 2)
                    {
                        //If the paint tool is selected, create a new block at the current block handle position
                        AddBlock(LevelEditor.CurrentHandlePosition);
                    }
                }
            }

            //If we press escape we want to automatically deselect our own painting or erasing tools
            if (Event.current.type == EventType.KeyDown &&
                Event.current.keyCode == KeyCode.Escape)
            {
                ToolMenu.SelectedTool = 0;
            }

            //Add our controlId as default control so it is being picked instead of Unitys default SceneView behaviour
            HandleUtility.AddDefaultControl(controlId);
        }

        //Create a new basic cube at the given position
        public static void AddBlock(Vector3 position)
        {

            Vector3 size = LevelGenerator.size;
            Material mat;
            if (LevelGenerator.mat == null)
            {
                mat = new Material(Shader.Find("Specular"));
            }
            else
                mat = LevelGenerator.mat;
            if (LevelGenerator.selectedType == LevelGenerator.TYPE.Cube)
            {
                GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newCube.transform.parent = LevelParent;
                newCube.transform.position = position;
                newCube.transform.localScale = size;
             
              //  newCube.layer = LayerMask.NameToLayer("Level");
                newCube.GetComponent<Renderer>().material = mat;
                Undo.RegisterCreatedObjectUndo(newCube, "Add Cube");
            }
            else if(LevelGenerator.selectedType == LevelGenerator.TYPE.Sphere)
            {
                GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newSphere.transform.parent = LevelParent;
                newSphere.transform.position = position;
                newSphere.transform.localScale = size;
                newSphere.AddComponent<SphereCollider>();
              
                //newSphere.layer = LayerMask.NameToLayer("Default");
                newSphere.GetComponent<Renderer>().material = mat;
                Undo.RegisterCreatedObjectUndo(newSphere, "Add Cube");
            }
            else if (LevelGenerator.selectedType == LevelGenerator.TYPE.Capsule)
            {
                GameObject newCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                newCapsule.transform.parent = LevelParent;
                newCapsule.transform.position = position;
                newCapsule.transform.localScale = size;
                newCapsule.AddComponent<CapsuleCollider>();
             
             //   newCapsule.layer = LayerMask.NameToLayer("Level");
                Undo.RegisterCreatedObjectUndo(newCapsule, "Add Cube");
                newCapsule.GetComponent<Renderer>().material = mat;
            }
            else if (LevelGenerator.selectedType == LevelGenerator.TYPE.Cylinder)
            {
                GameObject newCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                newCylinder.transform.parent = LevelParent;
                newCylinder.transform.position = position;
                newCylinder.transform.localScale = size;
                newCylinder.AddComponent<BoxCollider>();
             
              //  newCylinder.layer = LayerMask.NameToLayer("Level");
                newCylinder.GetComponent<Renderer>().material = mat;
                Undo.RegisterCreatedObjectUndo(newCylinder, "Add Cube");
            }
            else if (LevelGenerator.selectedType == LevelGenerator.TYPE.Prefab)
            {
                GameObject newGameObject = PrefabUtility.InstantiatePrefab(LevelGenerator.source) as  GameObject;
                newGameObject.transform.parent = LevelParent;
                newGameObject.transform.position = position;
                newGameObject.transform.localScale = size;
                newGameObject.AddComponent<BoxCollider>();
            
               // newGameObject.layer = LayerMask.NameToLayer("Level");
                
                Undo.RegisterCreatedObjectUndo(newGameObject, "Add Cube");
            }





            //   newCube.GetComponent<Renderer>().material = (Material)AssetDatabase.LoadAssetAtPath("Assets/Shared Resources/Materials/Grid@GreenBlue.mat", typeof(Material));

            //Make sure a proper Undo/Redo step is created. This is a special type for newly created objects


            //Mark the scene as dirty so it is being saved the next time the user saves
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }

        //Remove a gameobject that is close to the given position
        public static void RemoveBlock(Vector3 position)
        {
            for (int i = 0; i < LevelParent.childCount; ++i)
            {
                float distanceToBlock = Vector3.Distance(LevelParent.GetChild(i).transform.position, position);
                if (distanceToBlock < 0.1f)
                {
                    //Use Undo.DestroyObjectImmediate to destroy the object and create a proper Undo/Redo step for it
                    Undo.DestroyObjectImmediate(LevelParent.GetChild(i).gameObject);

                    //Mark the scene as dirty so it is being saved the next time the user saves
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                    return;
                }
            }
        }

        //I will use this type of function in many different classes. Basically this is useful to 
        //be able to draw different types of the editor only when you are in the correct scene so we
        //can have an easy to follow progression of the editor while hoping between the different scenes
        static bool IsInCorrectLevel()
        {
            return SceneManager.GetActiveScene().name == "SampleScene";
        }
    }
}
#endif