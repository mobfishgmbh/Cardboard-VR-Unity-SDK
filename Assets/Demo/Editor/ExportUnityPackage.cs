using System;
using UnityEditor;
using UnityEngine;

namespace MobfishCardboardDemo
{
    public class ExportUnityPackage
    {
        [MenuItem("mobfishCardboard/ExportPackage")]
        private static void ExportPackage()
        {
            string fileLocation = EditorUtility.SaveFilePanel(
                "Export Unity Package", String.Empty, "mobfishCardboard",
                "unitypackage");
            Debug.Log("Ready to export, location=" + fileLocation);
            AssetDatabase.ExportPackage("Assets/Cardboard", fileLocation,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            Debug.Log("Package exported, location=" + fileLocation);
        }
    }
}