using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace Halodi.PackageCreator
{
    internal class PublicationController
    {
        private static readonly string EDITOR_PREFS_PREFIX = "com.halodi.halodi-unity-package-creator.";



        internal static PublicationModel LoadModel()
        {
            PublicationModel model = new PublicationModel();

            string DefaultNPMExecutable = "";

#if UNITY_EDITOR_WIN
            DefaultNPMExecutable = "C:\\Program Files\\nodejs\\npm.cmd";
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


        internal static string Publish(PublicationModel model)
        {

            if (PackageConfigurationController.PackageIsInitialized())
            {
                using (System.Diagnostics.Process npm = new System.Diagnostics.Process())
                {

                    npm.StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = model.NPMExecutable,
                        Arguments = "publish --registry " + model.RegisteryURL,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WorkingDirectory = PackageConfigurationController.PackageFolderOnDisk()
                    };

                    npm.Start();

                    string outputPath = FileUtil.GetUniqueTempPathInProject();
                    StreamWriter writer = new StreamWriter(outputPath, false);

                    
                    while (!npm.StandardError.EndOfStream)
                    {
                        writer.WriteLine(npm.StandardError.ReadLine());
                    }
                    while (!npm.StandardOutput.EndOfStream)
                    {
                        writer.WriteLine(npm.StandardOutput.ReadLine());
                    }

                    writer.Close();

                    return outputPath;
                }

            }
            else
            {
                return "Cannot publish package. Package not initialized.";
            }
        }

    }
}