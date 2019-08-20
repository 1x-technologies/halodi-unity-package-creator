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
                manifest.repositiory.type =  EditorGUILayout.TextField("\ttype: ", manifest.repositiory.type);
                manifest.repositiory.url =  EditorGUILayout.TextField("\turl: ", manifest.repositiory.url);

                manifest.license = EditorGUILayout.TextField("License: ", manifest.license);

                EditorGUILayout.LabelField("Author");
                manifest.author.name =  EditorGUILayout.TextField("\tname: ", manifest.author.name);
                manifest.author.email =  EditorGUILayout.TextField("\temail: ", manifest.author.email);
                manifest.author.url =  EditorGUILayout.TextField("\turl: ", manifest.author.url);

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
            EditorApplication.delayCall += () => 
            { 
                PackageConfigurationView view = EditorWindow.GetWindow<PackageConfigurationView>(true, "Package Configuration", true);  
                
            };
        }

    }
}
