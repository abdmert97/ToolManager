using System;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class WorldToScreen : MonoBehaviour
    {
        public RectTransform destination;
        public Transform startPosition;
        public GameObject UIelement;
        public RectTransform UI;
        private RectTransform created;
        private void Start()
        {
            created = Instantiate(UIelement, startPosition.position, Quaternion.identity,UI.transform).GetComponent<RectTransform>();
          
            ToolManager.Instance.GetTool<VectorOperation>().SmoothVectorLerp(startPosition.position+UI.position,
                    destination.position
                    ,100f,true,move);
        
        }

        void move(Vector3 pos)
        {
            created.position = pos;
        }
    }
}
