using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class HalodiAddSampleView : EditorWindow
    {
        PackageManifest.Sample sample = null;
        PackageManifest manifest = null;

        void OnEnable()
        {
            sample = new PackageManifest.Sample();
        }

        void OnDisable()
        {
            manifest = null;
        }

        void OnGUI()
        {
            if(manifest != null)
            {
                EditorGUILayout.LabelField("New sample for " + manifest.displayName);
                sample.displayName = EditorGUILayout.TextField("Display name: ", sample.displayName);
                sample.description = EditorGUILayout.TextField("Description: ", sample.description);
                sample.path = EditorGUILayout.TextField("Path: ", sample.path);

                if (GUILayout.Button("Create"))
                {
                    OnClickCreate();
                }
               
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                    GUIUtility.ExitGUI();
                }
            }

        }

        private void OnClickCreate()
        {
            if(!HalodiNewPackageController.ValidateFolderName(sample.path))
            {
                EditorUtility.DisplayDialog("Invalid sample path", "Please specify a valid sample paths. Valid sample paths start with a letter, and only contains letters, numbers, underscores and hyphens.", "Close");
                return;
            }

            if(sample.displayName.Length == 0)
            {
                EditorUtility.DisplayDialog("Invalid display name", "Please specify a display name.", "Close");
                return;
            }

            sample.path = Paths.PackageSamplesFolder + "/" + sample.path;   // No paths.combine, written to file in specific format with slash

            HalodiPackageCreatorController.AddSample(manifest, sample);

            Close();
            GUIUtility.ExitGUI();
        }

        public static void AddSample(PackageManifest package)
        {
            HalodiAddSampleView addSampleView = EditorWindow.GetWindow<HalodiAddSampleView>(true, "Add sample", true);
            addSampleView.manifest = package;
        }
    }

}