using System.Collections.Generic;

namespace ToolsMert
{
    [System.Serializable]
    public class LevelSave
    {
        public string assetPath;
        public List<float> posX;
        public List<float> posY;
        public List<float> posZ;
        public List<float> scaleX;
        public List<float> scaleY;
        public List<float> scaleZ;
        public List<float> rotationX;
        public List<float> rotationY;
        public List<float> rotationZ;

        public LevelSave()
        {
                        
            posX = new List<float>();
            posY = new List<float>();
            posZ = new List<float>();
            
            scaleX = new List<float>();
            scaleY = new List<float>();
            scaleZ = new List<float>();
            
            rotationX = new List<float>();
            rotationY = new List<float>();
            rotationZ = new List<float>();
        }
    }
}

