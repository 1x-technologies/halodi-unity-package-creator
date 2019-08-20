using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    public class PackageConfigurationWindow : EditorWindow
    {
        PackageManifest manifest = new PackageManifest();

        private EditorApplication.CallbackFunction ApplicationUpdateCallback;


        void OnGUI()
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
            manifest.author.url =  EditorGUILayout.TextField("\temail: ", manifest.author.url);

            EditorGUILayout.LabelField("Publish configuration");
            manifest.publishConfig.registry =  EditorGUILayout.TextField("\tregistry: ", manifest.publishConfig.registry);

            if (GUILayout.Button("Create"))
            {
                OnClickCreate();
            }
        }

        void OnClickCreate()
        {
            manifest.name = manifest.name.Trim();

            if (string.IsNullOrEmpty(manifest.name))
            {
                EditorUtility.DisplayDialog("Invalid package name", "Please specify a valid package name.", "Close");
                return;
            }
            else
            {
                Close();
                InitializePackageStructure.CreatePackage(manifest);
                GUIUtility.ExitGUI();
            }
        }


        public static void ShowWindow()
        {
            EditorApplication.delayCall += () => EditorWindow.GetWindow<PackageConfigurationWindow>(true, "Package Configuration", true);  
        }

    }
}
