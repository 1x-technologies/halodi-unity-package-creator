using UnityEditor;
using UnityEngine;

namespace Halodi
{
    namespace Editor
    {
        public class PackageConfigurationWindow : EditorWindow
        {
            PackageManifest manifest = new PackageManifest();
 


            void OnGUI()
            {
                EditorGUILayout.LabelField("Intialize new package");
                manifest.name = EditorGUILayout.TextField("Name: ", manifest.name);
                manifest.version = EditorGUILayout.TextField("Version: ", manifest.version);
                manifest.displayName = EditorGUILayout.TextField("Display name: ", manifest.displayName);
                manifest.description = EditorGUILayout.TextField("Description: ", manifest.description);
                manifest.unity = EditorGUILayout.TextField("Unity version: ", manifest.unity);

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
        }
    }
}