using UnityEngine;

namespace ToolsMert.Editor
{
    public class Cube : MonoBehaviour
    {

        public float val;
        void Start()
        {
            GenerateColor();
        }

        public void GenerateColor()
        {
            GetComponent<Renderer>().sharedMaterial.color = Random.ColorHSV();
        }

        public void Reset()
        {
            GetComponent<Renderer>().sharedMaterial.color = Color.white;
        }

    }
}