using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal class PublicationController
{
    private static readonly string EDITOR_PREFS_PREFIX = "com.halodi.halodi-unity-package-creator.";



    internal static PublicationModel LoadModel()
    {
        PublicationModel model = new PublicationModel();

        string DefaultNPMExecutable = "";

        #if UNITY_EDITOR_WIN
            DefaultNPMExecutable = "C:\Program Files\nodejs\node_modules\npm.cmd";
        #elif UNITY_EDITOR_LINUX
            DefaultNPMExecutable = "/usr/bin/npm";
        #elif UNITY_EDITOR_OSX
            DefaultNPMExecutable = "/usr/bin/npm";
        #endif



        model.NPMExecutable = EditorPrefs.GetString(EDITOR_PREFS_PREFIX + "npm-registry", DefaultNPMExecutable);
        model.RegisteryURL = EditorPrefs.GetString(EDITOR_PREFS_PREFIX + "registry-url", PublicationModel.DEFAULT_REGISTRY);
        
        return model;
    }    

    internal static void SaveModel(PublicationModel model)
    {
        EditorPrefs.SetString(EDITOR_PREFS_PREFIX + "npm-registry", model.NPMExecutable);
        EditorPrefs.SetString(EDITOR_PREFS_PREFIX + "registry-url", model.RegisteryURL);
    }


    internal static void Publish(PublicationModel model)
    {
        using(System.Diagnostics.Process npm = new System.Diagnostics.Process())
        {
            npm.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = model.NPMExecutable,
                Arguments = "--registry " + model.RegisteryURL,
                UseShellExecute = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            npm.OutputDataReceived += (sender, args) => Debug.Log(args.Data);
            npm.ErrorDataReceived += (sender, args) => Debug.LogWarning(args.Data);

            npm.Start();
            npm.BeginOutputReadLine();
            npm.BeginErrorReadLine();

        }


    }

}
