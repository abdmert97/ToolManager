using System;
using System.Collections;
using System.Reflection;
using ToolsMert;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools
{
    public class Test : MonoBehaviour
    {
        private BoundCalculator _boundCalculator;
        private Coroutine _coroutine;
        private MeshOperation _meshOperation;
        private ObjectPool _objectPool;
        private VectorOperation _vectorOperation;

        private ToolManager _toolManager;

        private GameObject testObject;
        // Start is called before the first frame update
        void Start()
        {
            testObject =  GameObject.CreatePrimitive(PrimitiveType.Cube);
            _toolManager = ToolManager.Instance;

            StartCoroutine(TestRoutine());
          
            
        }

        private IEnumerator TestRoutine()
        {
            Debug.Log("Bound calculator test started");
            BoundCalculatorTest();
            Debug.Log("Bound calculator test ended");
            yield return new WaitForSeconds(2);
            Debug.Log("Coroutine test started");
            StartCoroutine(CoroutineTest());
            yield return new WaitForSeconds(2);
         // Debug.Log("Mesh operation test started");
         // StartCoroutine(MeshOperationTest());
         // yield return new WaitForSeconds(2);
            Debug.Log("Object pool test started");
            StartCoroutine(ObjectPoolTest());
            Debug.Log("Vector operation test started");
            StartCoroutine(VectorOperationTest());

        }

        private IEnumerator VectorOperationTest()
        {
            _vectorOperation = _toolManager.GetTool<VectorOperation>();
            
            Vector3 target = transform.position + Vector3.back * 5;
            _vectorOperation.SmoothVectorLerp(testObject.transform.position,target,1,true,SetTransform);
            yield return  new WaitForSeconds(1);
            Debug.Log("Vector operation test ended");
        }

        void SetTransform(Vector3 v)
        {
            testObject.transform.position = v;
        }
        private IEnumerator ObjectPoolTest()
        {
             _objectPool = _toolManager.GetTool<ObjectPool>();

             _objectPool.poolObject = testObject;
              _objectPool.GetObjectFromPool();
             yield return new WaitForSeconds(1);
             
             _objectPool.GetObjectFromPool();
             yield return new WaitForSeconds(1);
        }

        private IEnumerator MeshOperationTest()
        {
              _meshOperation = _toolManager.GetTool<MeshOperation>();
            
             _meshOperation.CreateSphere();
             _meshOperation.CreateSphereIndices();
            // _meshOperation.CreateSquare();
            // _meshOperation.CreateSquareIndices();
             _meshOperation.CreateMesh();
              
              Debug.Log("Mesh operation test ended");
              yield return new WaitForSeconds(1);
        }

        private IEnumerator CoroutineTest()
        {
            _coroutine = _toolManager.GetTool<Coroutine>();
            
            Debug.Log("Delta move started");
            _coroutine.CoroutineDeltaFrame(e => e<2,moveDelta);
            Debug.Log("Delta move end");
            yield return new WaitForSeconds(2);
            
            Debug.Log("Fixed move started");
            _coroutine.CoroutineFixed(e => e<2,moveFixed);
            Debug.Log("Fixed move end");
            yield return new WaitForSeconds(2);

            Debug.Log("Delta move started");
            _coroutine.CoroutineWithWaitTime(2,moveDelta);
            Debug.Log("Delta move end");
            yield return new WaitForSeconds(2);

            Debug.Log("Fixed move started");
            _coroutine.CoroutineFixedWithWaitTime(2,moveFixed);
            Debug.Log("Fixed move end");

            
            Debug.Log("wait and Delta move started");
            _coroutine.CoroutineWaitSeconds(1, e => e < 2, moveDelta);
            Debug.Log("Delta move end");
            Debug.Log("Coroutine test ended");
        }

        void moveDelta()
        {
            testObject.transform.position += Time.deltaTime*Vector3.forward;
        }
        void moveFixed()
        {
            testObject.transform.position += Time.fixedDeltaTime*Vector3.forward;
        }
        private void BoundCalculatorTest()
        {
            _boundCalculator = _toolManager.GetTool<BoundCalculator>();
           Debug.Log(BoundCalculator.GetBounds(testObject).size);
           testObject.transform.localScale = BoundCalculator.SetUnitScale(testObject,2,2,2);
        }


        // Update is called once per frame
        void Update()
        {

        }

        
        
        void SetPosition(Vector3 v)
        {
            transform.position = v;
        }
        void SetRotation(Vector3 v)
        {
            transform.rotation = Quaternion.Euler(v);
        }
    }
}
