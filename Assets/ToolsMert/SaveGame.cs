using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ToolsMert;


public class SaveGame : EditorWindow
{

   static string inputPath = "/Level.txt";

    [MenuItem("Window/Load Level")]
    public static void ShowWindow()
    {
        GetWindow<SaveGame>("SaveGame");
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
    void OnGUI()
    {
        
        if (GUILayout.Button("Load Game"))
        {
            loadGame();
        }
    }
        /*  public static List<string> Save(GameObject level)
          {

              List<string> result = new List<string>();
              int count = level.transform.childCount;
              StreamWriter writer = new StreamWriter(path, true);
              for (int i = 0; i < count; i++)
              {
                  Transform obj = level.transform.GetChild(i);
                  LevelSave save = new LevelSave();
                  save.x = obj.localPosition.x;
                  save.y = obj.localPosition.y;
                  save.z = obj.localPosition.z;
                  string json = JsonUtility.ToJson(save);
                  writer.WriteLine(json);
                  result.Add(json);
              } 
              writer.Close();
              return result;
          }
          public static void Load(List<string> json)
          {
              int length = json.Count;
              StreamReader reader = new StreamReader(path);
              string jsoni=reader.ReadToEnd();

              for (int i = 0; i < 0; i++)
              {
                  LevelSave obj = JsonUtility.FromJson<LevelSave>(json[0]+json[1]);
                  GameObject pre= GameObject.CreatePrimitive(PrimitiveType.Cube);
                  pre.transform.position = new Vector3(obj.x, obj.y, obj.z);
              }



              reader.Close();
          }
          */
        public static void saveGame(LevelSave level)
    {
        BinaryFormatter bf = new BinaryFormatter();

        string filePath = Application.dataPath  +inputPath ;
        Debug.Log(filePath);
        FileStream fs = new FileStream(filePath, FileMode.Create);

        

        bf.Serialize(fs, level);

        fs.Close();
    }

    public static void loadGame()
    {
        string filePath = Application.dataPath  + inputPath ;
        Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filePath, FileMode.Open);

            LevelSave saveGameData = bf.Deserialize(fs) as LevelSave;

            fs.Close();


           GameObject parent = GameObject.Find("Level");
            if (parent == null)
            {
                GameObject par = new GameObject();
             parent = GameObject.Instantiate(par, Vector3.zero, Quaternion.identity);
            }
            parent.name = "Level";
            if (!saveGameData.assetPath.Equals(""))
            {
                GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(saveGameData.assetPath,typeof(GameObject));
                int count = saveGameData.posX.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject created = Instantiate(prefab, parent.transform, true);
                    created.transform.localPosition = new Vector3(saveGameData.posX[i], saveGameData.posY[i], saveGameData.posZ[i]);
                    created.transform.localRotation = Quaternion.Euler(new Vector3(saveGameData.rotationX[i], saveGameData.rotationY[i], saveGameData.rotationZ[i]));
                    created.transform.localScale = new Vector3(saveGameData.scaleX[i], saveGameData.scaleY[i], saveGameData.scaleZ[i]);
                
                }
            }
            else
            {
                int count = saveGameData.posX.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject cube  = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.SetParent(parent.transform);
                    cube.transform.localPosition = new Vector3(saveGameData.posX[i], saveGameData.posY[i], saveGameData.posZ[i]);
                    cube.transform.localRotation = Quaternion.Euler(new Vector3(saveGameData.rotationX[i], saveGameData.rotationY[i], saveGameData.rotationZ[i]));
                    cube.transform.localScale = new Vector3(saveGameData.scaleX[i], saveGameData.scaleY[i], saveGameData.scaleZ[i]);
                
                }
            }
            

        }
      
    }
}
