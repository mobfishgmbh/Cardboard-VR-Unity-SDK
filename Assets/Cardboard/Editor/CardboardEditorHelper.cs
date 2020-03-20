#if UNITY_IOS
#define USING_IOS_XCODE

#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USING_IOS_XCODE
using UnityEditor.iOS.Xcode;

#endif

namespace MobfishCardboard.UEditor
{
    public static class CardboardEditorHelper
    {
        public static void PostProcessXCode(string path)
        {
            #if USING_IOS_XCODE
            Debug.Log("CardboardEditorHelper.PostProcessXCode() Start.");

            // Go get pbxproj file
            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            // PBXProject class represents a project build settings file,
            // here is how to read that in.
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);

            // This is the Xcode target in the generated project
            string target = proj.TargetGuidByName("Unity-iPhone");


            // If building with the non-bitcode version of the plugin, these lines should be uncommented.
            Debug.Log("CardboardEditorHelper.PostProcessXCode() Setting build property: ENABLE_BITCODE = NO");
            proj.AddBuildProperty(target, "ENABLE_BITCODE", "NO");

            // Write PBXProject object back to the file
            proj.WriteToFile(projPath);

            Debug.Log("CardboardEditorHelper.PostProcessXCode() finished");
            #endif
        }
    }
}