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
 
        private bool useGroupVersion;
        private string groupVersion;

        [MenuItem("Packages/Manage packages in project", false, 0)] //creates a new menu tab
        internal static void EditPackageConfiguration()
        {
            HalodiPackageCreatorView.ShowWindow();
        }

        void OnEnable()
        {
            packages = HalodiPackageCreatorController.LoadPackages();
            minSize = new Vector2(640, 320);

            useGroupVersion = PackageGroupConfiguration.IsUseGroupVersion();
            if(useGroupVersion)
            {
                groupVersion = PackageGroupConfiguration.GetGroupVersion();
            }    
            else
            {
                groupVersion = "0.0.0";
            }
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Packages in this project", EditorStyles.whiteLargeLabel);

            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            useGroupVersion = EditorGUILayout.ToggleLeft("Use common version for all packages in this project", useGroupVersion);

            if(EditorGUI.EndChangeCheck())
            {
                if(!useGroupVersion)
                {
                    PackageGroupConfiguration.UnsetGroupVersion();
                }
            }
            

            if(useGroupVersion)
            {
                EditorGUILayout.BeginHorizontal();
                groupVersion = EditorGUILayout.TextField("Group version: ", groupVersion);
                if(GUILayout.Button("Apply"))
                {
                    ApplyGroupVersion();
                }
                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.Space();

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

                if(GUILayout.Button("Add sample"))
                {
                    AddSample(package);
                }
                if(GUILayout.Button("Pack"))
                {
                    Pack(package);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if( GUILayout.Button("Publish"))
            {
                PublishPackages();
            }

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

        private void ApplyGroupVersion()
        {
            
            if(PackageGroupConfiguration.IsValidVersion(groupVersion))
            {
                PackageGroupConfiguration.SetGroupVersion(groupVersion);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid version", "Version is not a semantic version (major.minor.patch).", "OK");
                if(PackageGroupConfiguration.IsUseGroupVersion())
                {
                    groupVersion = PackageGroupConfiguration.GetGroupVersion();
                }
                else
                {
                    groupVersion = "0.0.0";
                }
            }
        }


        private void PublishPackages()
        {
            PublicationView.PublishPackages(packages);
            CloseWindow();
        }

        private void SelectPackage(PackageManifest package)
        {
            UnityEngine.Object instance = HalodiPackageCreatorController.GetPackageManifestObject(package);
            Selection.activeObject = instance;
            CloseWindow();
        }

        private void AddSample(PackageManifest package)
        {

            HalodiAddSampleView.AddSample(package);
            CloseWindow();
        }

        private void Pack(PackageManifest package)
        {
            PublicationController.Pack(package);
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
            EditorWindow.GetWindow<HalodiPackageCreatorView>(true, "Halodi package creator", true);  
        }

    }
}
