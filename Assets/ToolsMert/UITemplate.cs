using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class UITemplate : MonoBehaviour
    {
        public GameObject canvas;


        public void CloseCanvas()
        {
            canvas.SetActive(false);
        }

        public void OpenCanvas()
        {
            canvas.SetActive(true);
        }
       

 
    }
}
