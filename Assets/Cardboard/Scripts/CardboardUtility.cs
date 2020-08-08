using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardUtility
    {
        #if UNITY_IOS
        public const string DLLName = "__Internal";
        public const string DLLName_UNITYJNI = "__Internal";
        #elif UNITY_ANDROID
        public const string DLLName = "cardboard_api";
        public const string DLLName_UNITYJNI = "cardboard_unity_jni";
        #endif


        public const int kResolution = 40;
        public const string KEY_CARDBOARD_CAMERA_SCANNED = "mobfishCardboardCamScanned";

        //https://github.com/googlevr/cardboard/blob/master/sdk/qrcode/ios/device_params_helper.mm
        public const string defaultCardboardUrl = "http://g.co/cardboard";

        public static CardboardMesh CreateMockupCardboardMesh(CardboardEye eye)
        {
            CardboardMesh result;

            if (eye == CardboardEye.kLeft)
            {
                result = new CardboardMesh()
                {
                    vertices = new[] {-0.9f, -0.9f, -0.9f, 0.9f, -0.1f, -0.9f, -0.1f, 0.9f},
                    n_vertices = 4,
                    indices = new[] {0, 1, 2, 3},
                    n_indices = 4,
                    uvs = new[] {0f, 0f, 0f, 1f, 1f, 0f, 1f, 1f}
                };
            }
            else
            {
                result = new CardboardMesh()
                {
                    vertices = new[] {0.1f, -0.9f, 0.1f, 0.9f, 0.9f, -0.9f, 0.9f, 0.9f},
                    n_vertices = 4,
                    indices = new[] {0, 1, 2, 3},
                    n_indices = 4,
                    uvs = new[] {0f, 0f, 0f, 1f, 1f, 0f, 1f, 1f}
                };
            }

            return result;
        }

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

        public static string DeviceParamsToString(DeviceParams deviceParams)
        {
            if (deviceParams == null)
                return string.Empty;

            StringBuilder result = new StringBuilder();
            result.Append("Vendor: ");
            result.AppendLine(deviceParams.Vendor);
            result.Append("Model: ");
            result.AppendLine(deviceParams.Model);
            result.Append("InterLensDistance: ");
            result.AppendLine(deviceParams.InterLensDistance.ToString());
            result.Append("ScreenToLensDistance: ");
            result.AppendLine(deviceParams.ScreenToLensDistance.ToString());
            result.Append("TrayToLensDistance: ");
            result.AppendLine(deviceParams.TrayToLensDistance.ToString());

            return result.ToString();
        }

        public static float[] Matrix4x4ToArray(Matrix4x4 source)
        {
            int length = 16;

            float[] target = new float[length];

            for (int i = 0; i < length; i++)
            {
                target[i] = source[i];
            }

            return target;
        }

        public static Matrix4x4 ArrayToMatrix4x4(float[] array)
        {
            Matrix4x4 result = new Matrix4x4();

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }
            return result;
        }

        public static Matrix4x4 GetTransformTRSMatrix(Transform targetTransform)
        {
            return Matrix4x4.TRS(targetTransform.localPosition, targetTransform.localRotation,
                targetTransform.localScale);
        }

        public static Pose GetPoseFromTRSMatrix(Matrix4x4 transformMatrix)
        {
            return new Pose()
            {
                position = transformMatrix.GetColumn(3),
                rotation = Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1))
            };
        }

        public static Vector2Int GetAdjustedScreenResolution()
        {
            if (Screen.width >= Screen.height)
            {
                return new Vector2Int(Screen.width, Screen.height);
            }
            else
            {
                return new Vector2Int(Screen.height, Screen.width);
            }
        }

        public static int GetTargetFramerate()
        {
            if (Screen.currentResolution.refreshRate < 60)
                return 60;
            else
                return Screen.currentResolution.refreshRate;
        }
    }
}