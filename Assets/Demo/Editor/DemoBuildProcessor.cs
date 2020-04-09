using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

using MobfishCardboard.UEditor;

namespace MobfishCardboardDemo
{
    public class DemoBuildProcessor: IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            string path = report.summary.outputPath;
            CardboardEditorHelper.PostProcessXCode(path);
        }
    }


}