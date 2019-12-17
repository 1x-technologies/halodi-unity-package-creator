using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class PackageConfigurationView : EditorWindow
    {

        private bool initialized = false;

        PackageManifest manifest = null;

        private EditorApplication.CallbackFunction ApplicationUpdateCallback;


        void LoadPackageManifest()
        {
            
        }

        void OnEnable()
        {
            PackageManifest currentManifest = PackageConfigurationController.LoadManifest();

            if(currentManifest == null)
            {
                manifest = new PackageManifest();
                initialized = false;
            }
            else
            {
                manifest = currentManifest;
                initialized = true;
            }
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
                manifest.description = EditorGUILayout.TextField("Description: ", manifest.description);
                manifest.unity = EditorGUILayout.TextField("Unity version: ", manifest.unity);

                EditorGUILayout.LabelField("Repository");
                manifest.repository.type =  EditorGUILayout.TextField("\ttype: ", manifest.repository.type);
                manifest.repository.url =  EditorGUILayout.TextField("\turl: ", manifest.repository.url);

                manifest.license = EditorGUILayout.TextField("License: ", manifest.license);

                EditorGUILayout.LabelField("Author");
                manifest.author.name =  EditorGUILayout.TextField("\tname: ", manifest.author.name);
                manifest.author.email =  EditorGUILayout.TextField("\temail: ", manifest.author.email);
                manifest.author.url =  EditorGUILayout.TextField("\turl: ", manifest.author.url);

                EditorGUILayout.LabelField("Dependencies");
                if (GUILayout.Button("Add Dependency"))
                {
                    manifest.dependency_list.Add(new PackageManifest.Dependency());
                }

                foreach(PackageManifest.Dependency entry in manifest.dependency_list)
                {
                    if(!entry.markDeleted)
                    {
                        entry.name = EditorGUILayout.TextField("\t    Name:", entry.name);
                        entry.version = EditorGUILayout.TextField("\t    Version:", entry.version);
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Separator();
                        if(GUILayout.Button("Remove"))
                        {
                            entry.markDeleted = true;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.LabelField("Publish configuration");
                manifest.publishConfig.registry =  EditorGUILayout.TextField("\tregistry: ", manifest.publishConfig.registry);


                if(!initialized)
                {
                    if (GUILayout.Button("Create"))
                    {
                        OnClickUpdate();
                    }
                }
                else
                {
                    if (GUILayout.Button("Update"))
                    {
                        OnClickUpdate();
                    }
                }

                if (GUILayout.Button("Cancel"))
                {
                    Close();
                    GUIUtility.ExitGUI();
                }
            }
        }

        void OnClickUpdate()
        {
            manifest.name_space = manifest.name_space.Trim();
            manifest.package_name = manifest.package_name.Trim();

            if (!PackageConfigurationController.ValidateName(manifest.name_space) || !PackageConfigurationController.ValidateName(manifest.package_name))
            {
                EditorUtility.DisplayDialog("Invalid package name and/or namespace", "Please specify a valid name for the package and/or namespace. Valid names start with a letter, and only contains letters, numbers, underscores and hyphens.", "Close");
                return;
            }
            else
            {
                Close();
                PackageConfigurationController.UpdateOrCreatePackage(manifest);
                GUIUtility.ExitGUI();
            }
        }


        internal static void ShowWindow()
        {

            if(PackageConfigurationController.PackageIsInitialized())
            {
                Object manifestObject = PackageConfigurationController.GetPackageManifest();
                Selection.activeObject = manifestObject;
            }
            else
            {
                EditorApplication.delayCall += () => EditorWindow.GetWindow<PackageConfigurationView>(true, "Package Configuration", true);  
            }
        }

    }
}
