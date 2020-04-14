using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobfishCardboard;

namespace MobfishCardboardDemo.SceneTest
{
    public class MeshTestScript: MonoBehaviour
    {
        public TextAsset meshDataRaw;
        public MeshFilter meshFilter;

        private void Awake()
        {
            CardboardMesh cMeshData = JsonUtility.FromJson<CardboardMesh>(meshDataRaw.text);
            Mesh unityMesh = CardboardUtility.ConvertCardboardMesh_Triangle(cMeshData);
            meshFilter.mesh = unityMesh;

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}