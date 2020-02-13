using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshOperation : MonoBehaviour
    {
        private GameObject _meshObject;
        private Mesh _mesh;

        private Vector3[] _normals;
        private Camera _camera;
        
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> indices = new List<int>();
        private List<Vector2> textureCoord = new List<Vector2>();
        [Header("SquareVars")]
        [SerializeField] private int width = 100;
        [SerializeField] private int height = 100;
        [SerializeField] private float scale = 100;

        [Header("SphereVars")]
        
        [SerializeField] private float radius = 2;

        [SerializeField] private int stackCount = 100;

        [SerializeField] private int sectorCount = 300;
       

        void Awake()
        {
            _meshObject = new GameObject("Mesh");
            
            _meshObject.AddComponent<MeshFilter>();
            _meshObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
          
            _mesh = _meshObject.GetComponent<MeshFilter> ().mesh;
         
            _camera = Camera.main;
            _mesh.MarkDynamic();
            _normals = _mesh.normals;
            
          
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                DeformMesh();
            }
        }

        public void DeformMesh()
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            DrawTriangle(ray);
        }

        private void DrawTriangle(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                int index = hit.triangleIndex;
            
           
                Vector3[] vertices = _mesh.vertices;
                int[] triangles = _mesh.triangles;
                for (int i =0;i<1; i++)
                {
                    vertices[triangles[(index + i) * 3 + 0]] -= _normals[triangles[(index + i) * 3 + 0]].normalized;
                    vertices[triangles[(index + i) * 3 + 1]] -= _normals[triangles[(index + i) * 3 + 1]].normalized;
                    vertices[triangles[(index + i) * 3 + 2]] -= _normals[triangles[(index + i) * 3 + 2]].normalized;
                }
                _mesh.triangles = triangles;
                _mesh.vertices = vertices;
                _mesh.RecalculateNormals();
                // Do something with the object that was hit by the raycast.
            }
        }
        public Texture2D CreateTexture(int width, int height)
        {
		
		
            Texture2D texture = new Texture2D(width,height);
            texture.Apply();
            return texture;
        }
        public static Texture2D CreateColorGradient(Color[] colors, int width,
            int height,
            TextureWrapMode textureWrapMode = TextureWrapMode.Clamp, FilterMode filterMode = FilterMode.Point,
            bool isLinear = false, bool hasMipMap = false)
        {
            if (colors == null || colors.Length == 0)
            {
                Debug.LogError("No colors assigned");
                return null;
            }

            int length = colors.Length;
            if (colors.Length > 8)
            {
                Debug.LogWarning("Too many colors! maximum is 8, assigned: " + colors.Length);
                length = 8;
            }

            // build gradient from colors
            var colorKeys = new GradientColorKey[length];
            var alphaKeys = new GradientAlphaKey[length];

            float steps = length - 1f;
            for (int i = 0; i < length; i++)
            {
                float step = i / steps;
                colorKeys[i].color = colors[i];
                colorKeys[i].time = step;
                alphaKeys[i].alpha = colors[i].a;
                alphaKeys[i].time = step;
            }

            // create gradient
            Gradient gradient = new Gradient();
            gradient.SetKeys(colorKeys, alphaKeys);

            // create texture
            Texture2D outputTex = new Texture2D(width, height, TextureFormat.ARGB32, false, isLinear);
            outputTex.wrapMode = textureWrapMode;
            outputTex.filterMode = filterMode;

            // draw texture
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    outputTex.SetPixel(i, j, gradient.Evaluate((float)i / (float)width));
                }
           
            }
            outputTex.Apply(false);

            return outputTex;
        } // BuildGradientTexture
        
        public void CreateSquare()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    vertices.Add(new Vector3(j*scale,0,i*scale));
                    textureCoord.Add(new Vector2((float)j/width, 1-(float)i/height));
                }
            }
		
        }

        public void CreateSphere()
        {
            float x, y, z, xy; // vertex position
           
            float s, t; // vertex texCoord

            float sectorStep = 2 * Mathf.PI / sectorCount;
            float stackStep = Mathf.PI / stackCount;
            float sectorAngle, stackAngle;

            int normalCount = 0;
            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = Mathf.PI / 2 - i * stackStep; // starting from pi/2 to -pi/2
                xy = radius * Mathf.Cos(stackAngle); // r * cos(u)
                z = radius * Mathf.Sin(stackAngle); // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // the first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep; // starting from 0 to 2pi

                    // vertex position (x, y, z)
                    x = xy * Mathf.Cos(sectorAngle); // r * cos(u) * cos(v)
                    y = xy * Mathf.Sin(sectorAngle); // r * cos(u) * sin(v)
                    vertices.Add(new Vector3(x, y, z));



                    // vertex tex coord (s, t) range between [0, 1]
                    s = (float) j / sectorCount;
                    t = (float) i / stackCount;
                    textureCoord.Add(new Vector2(s, t));

                }
            }
        }
        public void CreateSphereIndices()
        {
            int k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = i * (sectorCount + 1); // beginning of current stack
                k2 = k1 + sectorCount + 1; // beginning of next stack

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding first and last stacks
                    // k1 => k2 => k1+1
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);
                    }

                    // k1+1 => k2 => k2+1
                    if (i != (stackCount - 1))
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }
                }
            }
        }

        public void CreateSquareIndices()
        {
            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width - 1; j++)
                {
                    indices.Add(j+width*i);
                    indices.Add(j+1+width*i);
                    indices.Add(j+width*(i+1));
				
                    indices.Add(j+width*(i+1));
                    indices.Add(j+1+width*i);
                    indices.Add(j+1+width*(i+1));
                }
            }
        }

        public void CreateMesh()
        {
	
            _mesh.Clear ();
            _mesh.vertices = vertices.ToArray();
            _mesh.triangles = indices.ToArray();
            _mesh.uv = textureCoord.ToArray();
		
            _mesh.Optimize();
            _mesh.RecalculateNormals();
            _normals = _mesh.normals;
            _meshObject.GetComponent<MeshRenderer>().material.SetFloat("Scale",scale);
            _meshObject.AddComponent<MeshCollider>();

        }
    }    
}
