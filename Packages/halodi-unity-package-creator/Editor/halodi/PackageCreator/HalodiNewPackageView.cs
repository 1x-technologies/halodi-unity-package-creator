using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class HalodiNewPackageView : EditorWindow
    {
        PackageManifest manifest = null;
        RegistrySelector registrySelector = null;

        string[] licenseList = null;

        int licenseIndex = 0;

        void OnEnable()
        {
            manifest = new PackageManifest();
            registrySelector = new RegistrySelector();
            licenseList = SPDXLicenseList.Load().ToStringArray();
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            if(manifest != null)
            {
                EditorGUILayout.LabelField("Package Configuration");
                manifest.name_space = EditorGUILayout.TextField("Namespace: ", manifest.name_space);
                manifest.package_name = EditorGUILayout.TextField("Name: ", manifest.package_name);
                manifest.version = EditorGUILayout.TextField("Version: ", manifest.version);
                manifest.displayName = EditorGUILayout.TextField("Display name: ", manifest.displayName);

                licenseIndex = EditorGUILayout.Popup("License: ", licenseIndex, licenseList);

                EditorGUILayout.LabelField("Description");
                manifest.description = EditorGUILayout.TextArea(manifest.description, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));


                EditorGUILayout.LabelField("Publication configuration");
                manifest.publishConfig.registry =  registrySelector.SelectRegistry("\t", manifest.publishConfig.registry);

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

        void OnClickCreate()
        {
            manifest.name_space = manifest.name_space.Trim();
            manifest.package_name = manifest.package_name.Trim();
            manifest.license = licenseList[licenseIndex];

            if (!HalodiNewPackageController.ValidateName(manifest.package_name))
            {
                EditorUtility.DisplayDialog("Invalid package name", "Please specify a valid name for the package. Valid names start with a letter, and only contains letters, numbers, underscores and hyphens.", "Close");
                return;
            }
            else if(!HalodiNewPackageController.ValidateNameSpace(manifest.name_space) )
            {
                EditorUtility.DisplayDialog("Invalid namespace", "Please specify a valid namespace for the package. Valid namespaces start and end with a letter, and only contains letters, numbers, underscores, hyphens and dots as separation characters.", "Close");
                return;

            }
            else if (!HalodiNewPackageController.ValidateVersion(manifest.version))
            {
                EditorUtility.DisplayDialog("Invalid version", "Version needs to be in the format MAJOR.MINOR.PATCH, with only numeric values allowed.", "Close");
                return;
            }
            else if (manifest.displayName.Trim().Length == 0)
            {
                EditorUtility.DisplayDialog("No display name set", "No display name for  " + manifest.package_name + " set. Please provide a display name", "Close");
                return;
            }
            else if(HalodiNewPackageController.PackageExists(manifest.package_name))
            {
                EditorUtility.DisplayDialog("Package already exists", "A package named " + manifest.package_name + " already exists. Please choose another name.", "Close");
                return;
            }
            else
            {

                EditorUtility.DisplayProgressBar("Creating package", "Creating new package " + manifest.displayName, 0.5f);

                
                try
                {
                    HalodiNewPackageController.CreatePackage(manifest);
                }
                catch(System.Exception e)
                {
                    Debug.LogError(e);
                }
                EditorUtility.ClearProgressBar();

                UnityEngine.Object instance = HalodiPackageCreatorController.GetPackageManifestObject(manifest);
                Selection.activeObject = instance;

                Close();
                GUIUtility.ExitGUI();
            }
        }


        internal static void ShowWindow()
        {
            EditorWindow.GetWindow<HalodiNewPackageView>(true, "Create new package", true);  
        }

    }
}
