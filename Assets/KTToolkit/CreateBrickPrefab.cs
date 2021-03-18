using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KTToolkit
{
    public class CreateBrickPrefab : MonoBehaviour
    {
        [SerializeField] private Transform prefabRoot;
        public void CreateBrick()
        {
            Instantiate(Resources.Load("Prefabs/SampleBrickNavMesh"), Vector3.zero, new Quaternion(0,0,0,1), prefabRoot);
        }
    }
}
