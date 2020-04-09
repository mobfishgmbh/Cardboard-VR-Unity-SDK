using UnityEngine;
using MobfishCardboard;
using System.IO;

namespace MobfishCardboardDemo
{
    public static class NativeDataExtract
    {
        public const string FOLDER_EXTRACT = "NativeExtract";
        public const string FILE_EYEMESH = "EyeMesh";
        public const string FILE_ENCODED_PROFILE_PARAMS = "EncodedProfileParams";

        public static string GetSavePath(string fileName)
        {
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, FOLDER_EXTRACT)))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, FOLDER_EXTRACT));
            }
            return Path.Combine(Application.persistentDataPath, FOLDER_EXTRACT, fileName);
        }

        public static void SaveFile(string fileNameRoot, string fileContent, string fileExtension = "json")
        {
            int increment = 0;
            string targetFilePath = GetSavePath(string.Concat(fileNameRoot, ".", fileExtension));
            while (File.Exists(targetFilePath))
            {
                targetFilePath =
                    GetSavePath(string.Concat(fileNameRoot, "_", increment.ToString("00"), ".", fileExtension));
                increment++;
            }

            File.WriteAllText(targetFilePath, fileContent);
        }

        public static void Save_MeshJson(CardboardMesh sourceMeshData)
        {
            CardboardMeshJsonClass target = new CardboardMeshJsonClass(sourceMeshData);
            SaveFile(FILE_EYEMESH, JsonUtility.ToJson(target, true));
        }

        public static void Save_EncodedParam(byte[] encodedParams, int pointerLength)
        {
            CardboardProfileParamsJsonClass target = new CardboardProfileParamsJsonClass(encodedParams, pointerLength);
            SaveFile(FILE_ENCODED_PROFILE_PARAMS, JsonUtility.ToJson(target, true));
        }
    }

    [System.Serializable]
    public class CardboardMeshJsonClass
    {
        public int[] indices;
        public int n_indices;
        public float[] vertices;
        public float[] uvs;
        public int n_vertices;

        public CardboardMeshJsonClass(CardboardMesh sourceMeshData)
        {
            indices = sourceMeshData.indices;
            n_indices = sourceMeshData.n_indices;
            vertices = sourceMeshData.vertices;
            uvs = sourceMeshData.uvs;
            n_vertices = sourceMeshData.n_vertices;
        }
    }

    public class CardboardProfileParamsJsonClass
    {
        public int pointerLength;
        public byte[] rawParamData;

        public CardboardProfileParamsJsonClass(byte[] rawData, int pLength)
        {
            pointerLength = pLength;
            rawParamData = rawData;
        }
    }
}