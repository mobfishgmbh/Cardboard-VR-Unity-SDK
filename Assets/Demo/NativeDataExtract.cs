using UnityEngine;
using MobfishCardboard;
using System.IO;

public static class NativeDataExtract
{
	public const string FOLDER_EXTRACT = "NativeExtract";
	public const string FILE_EYEMESH = "EyeMesh";
	public const string FILE_ENCODED_PROFILE_PARAMS = "EncodedProfileParams";

	public static string GetSavePath(string fileName)
	{
		return Path.Combine(Application.persistentDataPath, FOLDER_EXTRACT, fileName);
	}

	public static void SaveFile(string fileNameRoot, string fileContent, string fileExtension="json")
	{
		int increment = 0;
		string targetFilePath = GetSavePath(string.Concat(fileNameRoot, ".", fileExtension));
		while (File.Exists(targetFilePath))
		{
			targetFilePath = GetSavePath(string.Concat(fileNameRoot, "_", increment.ToString("00"), ".", fileExtension));
		}

		File.WriteAllText(targetFilePath, fileContent);
	}

	public static void Save_MeshJson(CardboardMesh sourceMeshData)
	{
		CardboardMeshJsonClass target = new CardboardMeshJsonClass(sourceMeshData);
		SaveFile(FILE_EYEMESH, JsonUtility.ToJson(target));
	}

	public static void Save_EncodedParam(byte[] encodedParams)
	{
		CardboardProfileParamsJsonClass target = new CardboardProfileParamsJsonClass(encodedParams);
		SaveFile(FILE_ENCODED_PROFILE_PARAMS, JsonUtility.ToJson(target));
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
	public byte[] rawParamData;

	public CardboardProfileParamsJsonClass(byte[] rawData)
	{
		rawParamData = rawData;
	}
}