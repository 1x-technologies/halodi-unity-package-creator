using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    public class EmbedPackageView : EditorWindow
    {
        private PackageManifest manifest;

        [MenuItem("Assets/Embed Package", false)]
        private static void EmbedPackageMenu()
        {
            PackageManifest manifest = HalodiPackageCreatorController.GetPackageManifest(Selection.activeObject, false);
            if (manifest != null)
            {
                EmbedPackage(manifest);
            }

        }

        [MenuItem("Assets/Embed Package", true)]
        private static bool EmbedPackageMenuValidation()
        {
            PackageManifest manifest = HalodiPackageCreatorController.GetPackageManifest(Selection.activeObject, false);

            if(manifest == null)
            {
                return false;
            }

            // Do not allow embedding already embedded packages
            if (manifest.IsEmbedded)
            {
                return false;
            }

            // Check if a VCS URL is set
            if (string.IsNullOrEmpty(manifest.repository.type) || string.IsNullOrEmpty(manifest.repository.url))
            {
                return false;
            }

            // Check if VSC URL is a git url
            if (manifest.repository.type != "git")
            {
                return false;
            }



            return true;
        }

        [MenuItem("Assets/Open package folder", false, 1)]
        private static void OpenPackage()
        {
            PackageManifest manifest = HalodiPackageCreatorController.GetPackageManifest(Selection.activeObject, false);
            if (manifest != null)
            {
                EditorUtility.RevealInFinder(Path.Combine(manifest.info.assetPath, Paths.PackageManifest));
            }

        }

        [MenuItem("Assets/Open package folder", true, 1)]
        private static bool OpenPackageValidation()
        {
            PackageManifest manifest = HalodiPackageCreatorController.GetPackageManifest(Selection.activeObject, false);
            return manifest != null;
        }

        void OnGUI()
        {
            if (manifest != null)
            {
                EditorGUILayout.LabelField("Display name: " + manifest.displayName);

                manifest.repository.url = EditorGUILayout.TextField("Git repository: ", manifest.repository.url);

                if (GUILayout.Button("Embed"))
                {
                    HalodiPackageCreatorController.EmbedPackageFromGit(manifest);
                    Exit();
                }

                if (GUILayout.Button("Cancel"))
                {
                    Exit();
                }

            }

        }
        private void Exit()
        {
            Close();
            GUIUtility.ExitGUI();
        }

        internal static void EmbedPackage(PackageManifest package)
        {
            if (package.IsEmbedded)
            {
                Debug.LogWarning("Cannot embed already embedded package");
            }
            else
            {
                EmbedPackageView embedView = EditorWindow.GetWindow<EmbedPackageView>(true, "Embed package", true);
                embedView.manifest = package;
            }

        }
    }
}