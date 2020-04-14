using System.Collections.Generic;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardUtility
    {
        public const string DLLName = "__Internal";
        public const int kResolution = 40;

        public static Mesh ConvertCardboardMesh_LineStrip(CardboardMesh sourceMesh)
        {
            //https://github.com/googlevr/cardboard/blob/master/sdk/distortion_mesh.cc
            Mesh result = new Mesh();

            //vertices
            List<Vector3> vertList = new List<Vector3>();
            for (int i = 0; i < sourceMesh.vertices.Length; i += 2)
            {
                vertList.Add(new Vector3(sourceMesh.vertices[i], sourceMesh.vertices[i + 1], 0));
            }
            result.SetVertices(vertList);

            //indices
            result.SetIndices(sourceMesh.indices, MeshTopology.LineStrip, 0);

            //uv
            List<Vector2> uvList = new List<Vector2>();
            for (int i = 0; i < sourceMesh.uvs.Length; i += 2)
            {
                uvList.Add(new Vector2(sourceMesh.uvs[i], sourceMesh.uvs[i + 1]));
            }
            result.SetUVs(0, uvList);

            return result;
        }

        public static Mesh ConvertCardboardMesh_Triangle(CardboardMesh sourceMesh)
        {
            //https://github.com/googlevr/cardboard/blob/master/sdk/distortion_mesh.cc
            Mesh result = new Mesh();

            //vertices
            List<Vector3> vertList = new List<Vector3>();
            for (int i = 0; i < sourceMesh.vertices.Length; i += 2)
            {
                vertList.Add(new Vector3(sourceMesh.vertices[i], sourceMesh.vertices[i + 1], 0));
            }
            result.SetVertices(vertList);

            //uv
            List<Vector2> uvList = new List<Vector2>();
            for (int i = 0; i < sourceMesh.uvs.Length; i += 2)
            {
                uvList.Add(new Vector2(sourceMesh.uvs[i], sourceMesh.uvs[i + 1]));
            }
            result.SetUVs(0, uvList);

            //indices
            List<int> newIndices = new List<int>();
            for (int i = 0; i < sourceMesh.indices.Length - 2; i++)
            {
                int indexA = sourceMesh.indices[i];
                int indexB = sourceMesh.indices[i + 1];
                int indexC = sourceMesh.indices[i + 2];

                if (indexA == indexB || indexB == indexC)
                    continue;

                if (i % 2 == 0)
                {
                    newIndices.Add(indexA);
                    newIndices.Add(indexB);
                    newIndices.Add(indexC);
                }
                else
                {
                    newIndices.Add(indexA);
                    newIndices.Add(indexC);
                    newIndices.Add(indexB);
                }
            }

            // result.triangles = newIndices.ToArray();
            result.SetIndices(newIndices.ToArray(), MeshTopology.Triangles, 0);

            return result;
        }
    }
}