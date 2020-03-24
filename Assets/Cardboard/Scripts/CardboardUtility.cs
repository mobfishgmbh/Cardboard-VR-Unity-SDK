using System.Collections.Generic;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardUtility
    {
        public const string DLLName = "__Internal";

        public static Mesh ConvertCardboardMesh(CardboardMesh sourceMesh)
        {
            //https://github.com/googlevr/cardboard/blob/master/sdk/distortion_mesh.cc
            Mesh result = new Mesh();

            //uv
            List<Vector2> uvList = new List<Vector2>();
            for (int i = 0; i < sourceMesh.uvs.Length; i+=2)
            {
                uvList.Add(new Vector2(sourceMesh.uvs[i], sourceMesh.uvs[i + 1]));
            }
            result.SetUVs(0, uvList);

            //vertices
            List<Vector3> vertList = new List<Vector3>();
            for (int i = 0; i < sourceMesh.n_vertices; i += 2)
            {
                vertList.Add(new Vector3(sourceMesh.vertices[i], sourceMesh.vertices[i + 1], 0));
            }
            result.SetVertices(vertList);

            result.SetIndices(sourceMesh.indices, MeshTopology.Triangles, 0);

            return result;
        }
    }
}