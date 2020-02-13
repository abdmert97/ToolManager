using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
     public class BoundCalculator : MonoBehaviour
    {
        public static Bounds GetBounds(GameObject[] parentTransform)
        {
            List<Renderer> rendererList = new List<Renderer>();
            for (int j = 0; j < parentTransform.Length; j++)
            {
                for (int k = 0; k < parentTransform[j].transform.childCount; k++)
                {
                    Transform transform = parentTransform[j].transform.GetChild(k);
                    Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
                    foreach (Renderer render in renderers)
                    {
                        rendererList.Add(render);
                    }
                }
            }
            int rendererCount = rendererList.Count;
            if (rendererCount == 0)
                return new Bounds();
            Bounds bounds = rendererList[0].bounds;
            for (int i = 1; i < rendererCount; i++)
            {
                bounds.Encapsulate(rendererList[i].bounds);
            }
            return bounds;
        }

        
        public static Bounds GetBounds(GameObject parentTransform)
        {
            List<Renderer> rendererList = new List<Renderer>();
            Renderer renderparent=parentTransform.GetComponent<Renderer>();
            if( renderparent )
                rendererList.Add(renderparent);
            for (int k = 0; k < parentTransform.transform.childCount; k++)
            {
                Transform transform = parentTransform.transform.GetChild(k);
                Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer render in renderers)
                {
                    rendererList.Add(render);
                }
            }

            int rendererCount = rendererList.Count;
            if (rendererList == null || rendererCount == 0)
                return new Bounds();
            Bounds bounds = rendererList[0].bounds;
            for (int i = 1; i < rendererCount; i++)
            {
                bounds.Encapsulate(rendererList[i].bounds);
            }
      
            return bounds;
        }
        public static Vector3 SetUnitScale(GameObject gameObject, float x = 1,float y = -1, float z = 1)
        {
            var bounds = GetBounds(gameObject);
      
            var originalScale = gameObject.transform.localScale;
            float scaleX = (x / bounds.size.x);
            float scaleZ = (z/ bounds.size.z);
            float scaleY = y == -1 ? Mathf.Min(scaleX, scaleZ): y;
            return new Vector3(scaleX * originalScale.x, scaleY * originalScale.y, scaleZ * originalScale.z);
        }
    }
}

