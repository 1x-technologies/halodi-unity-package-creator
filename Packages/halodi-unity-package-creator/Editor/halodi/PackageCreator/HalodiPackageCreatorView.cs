using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class HalodiPackageCreatorView : EditorWindow
    {
        List<PackageManifest> packages = null;
        Vector2 scrollPos;
 
        [MenuItem("Packages/Manage packages in project")] //creates a new menu tab
        internal static void EditPackageConfiguration()
        {
            HalodiPackageCreatorView.ShowWindow();
        }

        void OnEnable()
        {
            packages = HalodiPackageCreatorController.LoadPackages();
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Packages in this project");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            
            foreach (PackageManifest package in packages)
            {
                GUIStyle boxStyle = new GUIStyle();
                boxStyle.padding = new RectOffset(10, 10, 0, 0);
                
                EditorGUILayout.BeginHorizontal(boxStyle);
                EditorGUILayout.LabelField(package.displayName);
                if(GUILayout.Button("Edit"))
                {
                    SelectPackage(package);
                }

                if(GUILayout.Button("Publish"))
                {
                    PublishPackage(package);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New package"))
            {
                NewPackage();
            }

            if (GUILayout.Button("Close"))
            {
                CloseWindow();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void PublishPackage(PackageManifest package)
        {
            PublicationView.PublishPackage(package);
            CloseWindow();
        }

        private void SelectPackage(PackageManifest package)
        {
            UnityEngine.Object instance = HalodiPackageCreatorController.GetPackageManifestObject(package);
            Selection.activeObject = instance;
            CloseWindow();
        }

        private void CloseWindow()
        {
            Close();
            GUIUtility.ExitGUI();
        }

        private void NewPackage()
        {
            HalodiNewPackageView.ShowWindow();
            CloseWindow();
        }

        internal static void ShowWindow()
        {
            EditorApplication.delayCall += () => EditorWindow.GetWindow<HalodiPackageCreatorView>(true, "Halodi package creator", true);  
        }

    }
}
