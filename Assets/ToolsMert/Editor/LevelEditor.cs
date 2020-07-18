using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace ToolsMert.Editor
{
    [InitializeOnLoad]
    public class LevelEditor : UnityEditor.Editor
    {
        public static Vector3 CurrentHandlePosition = Vector3.zero;
        public static bool IsMouseInValidArea = false;

        static Vector3 m_OldHandlePosition = Vector3.zero;

        static LevelEditor()
        {
            //The OnSceneGUI delegate is called every time the SceneView is redrawn and allows you
            //to draw GUI elements into the SceneView to create in editor functionality
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            bool isLevelEditorEnabled = EditorPrefs.GetBool("IsLevelEditorEnabled", true);

            //Ignore this. I am using this because when the scene GameE06 is opened we haven't yet defined any On/Off buttons
            //for the cube handles. That comes later in E07. This way we are forcing the cube handles state to On in this scene
            isLevelEditorEnabled = true;
      

            if (ToolMenu.SelectedTool ==0)
            {
                return;
            }
        
            UpdateHandlePosition(sceneView.in2DMode);
            UpdateIsMouseInValidArea(sceneView.position);
            UpdateRepaint();

       
        }

        //I will use this type of function in many different classes. Basically this is useful to 
        //be able to draw different types of the editor only when you are in the correct scene so we
        //can have an easy to follow progression of the editor while hoping between the different scenes
   

        static void UpdateIsMouseInValidArea(Rect sceneViewRect)
        {
            //Make sure the cube handle is only drawn when the mouse is within a position that we want
            //In this case we simply hide the cube cursor when the mouse is hovering over custom GUI elements in the lower
            //are of the sceneView which we will create in E07
            bool isInValidArea = Event.current.mousePosition.y < sceneViewRect.height - 35;

            if (isInValidArea != IsMouseInValidArea)
            {
                IsMouseInValidArea = isInValidArea;
                SceneView.RepaintAll();
            }
        }

        static void UpdateHandlePosition(bool is2D)
        {
            if (Event.current == null)
            {
                return;
            }

            Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);

            if(!is2D)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 offset = Vector3.zero;
                    Vector3 size = LevelGenerator.size;
                    if (EditorPrefs.GetBool("SelectBlockNextToMousePosition", true) == true)
                    {
                        offset = hit.normal;
                        offset.x *= LevelGenerator.size.x;
                        offset.y *= LevelGenerator.size.y;
                        offset.z *= LevelGenerator.size.z;
                    }

                    if (LevelGenerator.perfectFit)
                    {
                        MeshRenderer renderer = hit.transform.GetComponent<MeshRenderer>();

                        if (renderer == null)
                        {
                            renderer = hit.transform.GetComponentInChildren<MeshRenderer>();
                        }

                        if (renderer == null)
                        {
                            return;
                        }
                        Vector3 hitMidPoint = renderer.bounds.extents;
                        hitMidPoint.x *= hit.normal.x;
                        hitMidPoint.y *= hit.normal.y;
                        hitMidPoint.z *= hit.normal.z;
                        CurrentHandlePosition = hit.transform.position+hitMidPoint + offset/2;
                    }
                    else
                    {
                        CurrentHandlePosition.x = hit.point.x - hit.normal.x * 0.001f + offset.x/2;
                        CurrentHandlePosition.y = hit.point.y - hit.normal.y * 0.001f + offset.y/2;
                        CurrentHandlePosition.z = hit.point.z - hit.normal.z * 0.001f + offset.z/2; 
                       
                    }
                  
              
                    //CurrentHandlePosition += new Vector3(size/2, size/2, size/2);

                }
                DrawCubeDrawPreview();
            }
        
            else
            {

                Vector3 position;
                position = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(Event.current.mousePosition);
                //This makes the mouse position adjust so it is relative to the world position
                position.y = -(position.y - (2 * SceneView.currentDrawingSceneView.camera.transform.position.y));
                CurrentHandlePosition.x = Mathf.Floor(position.x);
                CurrentHandlePosition.y = Mathf.Floor(position.y);
                CurrentHandlePosition.z = 0;
                CurrentHandlePosition += new Vector3(0.5f, 0.5f, LevelGenerator.posZ);

            }
            DrawSquare(CurrentHandlePosition);

        }

        static void UpdateRepaint()
        {
            //If the cube handle position has changed, repaint the scene
            if (CurrentHandlePosition != m_OldHandlePosition)
            {
                SceneView.RepaintAll();
                m_OldHandlePosition = CurrentHandlePosition;
            }
        }

        static void DrawCubeDrawPreview()
        {
            if (IsMouseInValidArea == false)
            {
                return;
            }


      
       
            DrawHandlesCube(CurrentHandlePosition);
        }

        static void DrawHandlesCube(Vector3 center)
        {
            float  sizeX = LevelGenerator.size.x / 2;
            float  sizeY = LevelGenerator.size.y / 2;
            float  sizeZ = LevelGenerator.size.z / 2;
            Handles.color = new Color(255, 255, 0);
            Vector3 p1 = center + Vector3.up * sizeY + Vector3.right * sizeX + Vector3.forward * sizeZ;
            Vector3 p2 = center + Vector3.up * sizeY + Vector3.right * sizeX - Vector3.forward * sizeZ;
            Vector3 p3 = center + Vector3.up * sizeY - Vector3.right * sizeX - Vector3.forward * sizeZ;
            Vector3 p4 = center + Vector3.up * sizeY - Vector3.right * sizeX + Vector3.forward * sizeZ;

            Vector3 p5 = center - Vector3.up * sizeY + Vector3.right * sizeX + Vector3.forward * sizeZ;
            Vector3 p6 = center - Vector3.up * sizeY + Vector3.right * sizeX - Vector3.forward * sizeZ;
            Vector3 p7 = center - Vector3.up * sizeY - Vector3.right * sizeX - Vector3.forward * sizeZ;
            Vector3 p8 = center - Vector3.up * sizeY - Vector3.right * sizeX + Vector3.forward * sizeZ;

            //You can use Handles to draw 3d objects into the SceneView. If defined properly the
            //user can even interact with the handles. For example Unitys move tool is implemented using Handles
            //However here we simply draw a cube that the 3D position the mouse is pointing to
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p4);
            Handles.DrawLine(p4, p1);

            Handles.DrawLine(p5, p6);
            Handles.DrawLine(p6, p7);
            Handles.DrawLine(p7, p8);
            Handles.DrawLine(p8, p5);

            Handles.DrawLine(p1, p5);
            Handles.DrawLine(p2, p6);
            Handles.DrawLine(p3, p7);
            Handles.DrawLine(p4, p8);
        }
        static void DrawSquare(Vector3 center)
        {
            if (IsMouseInValidArea == false)
            {
                return;
            }
            Handles.color = new Color(255,0,0);
            float  sizeX = LevelGenerator.size.x / 2;
            float  sizeY = LevelGenerator.size.y / 2;
            float  sizeZ = LevelGenerator.size.z / 2;
            Vector3 p2 = center + Vector3.up * sizeY + Vector3.right * sizeX - Vector3.forward * sizeZ;
            Vector3 p3 = center + Vector3.up * sizeY - Vector3.right * sizeX - Vector3.forward * sizeZ;

            Vector3 p6 = center - Vector3.up * sizeY + Vector3.right * sizeX - Vector3.forward * sizeZ;
            Vector3 p7 = center - Vector3.up * sizeY - Vector3.right * sizeX - Vector3.forward * sizeZ;
       

            //You can use Handles to draw 3d objects into the SceneView. If defined properly the
            //user can even interact with the handles. For example Unitys move tool is implemented using Handles
            //However here we simply draw a cube that the 3D position the mouse is pointing to
            // Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p6, p7);
            Handles.DrawLine(p2, p6);
            Handles.DrawLine(p3, p7);
    
        }
    }
}
#endif