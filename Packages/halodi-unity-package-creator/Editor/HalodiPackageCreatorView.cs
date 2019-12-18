using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class HalodiPackageCreatorView : EditorWindow
    {
        HalodiPackages packages = null;
        Vector2 scrollPos;
 
        [MenuItem("Halodi/Packages")] //creates a new menu tab
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
            EditorGUILayout.BeginScrollView(scrollPos);

            
            foreach (HalodiPackage package in packages.packages)
            {
                GUIStyle boxStyle = new GUIStyle();
                boxStyle.padding = new RectOffset(10, 10, 0, 0);
                
                EditorGUILayout.BeginHorizontal(boxStyle);
                EditorGUILayout.LabelField(package.PackageName);
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
            if (GUILayout.Button("Close"))
            {
                CloseWindow();
            }
        }

        private void PublishPackage(HalodiPackage package)
        {
            throw new NotImplementedException();
        }

        private void SelectPackage(HalodiPackage package)
        {
            UnityEngine.Object instance = HalodiPackageCreatorController.GetPackageManifest(package);
            Selection.activeObject = instance;
            CloseWindow();
        }

        private void CloseWindow()
        {
            Close();
            GUIUtility.ExitGUI();
        }

        internal static void ShowWindow()
        {
            EditorApplication.delayCall += () => EditorWindow.GetWindow<HalodiPackageCreatorView>(true, "Halodi package creator", true);  
        }

    }
}
