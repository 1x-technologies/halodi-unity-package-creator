using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi
{
    namespace Editor
    {





        [InitializeOnLoad]
        public class InitializePackageStructure
        {
            static InitializePackageStructure()
            {
                TextAsset halodiPackage = AssetDatabase.LoadAssetAtPath(Path.Combine(Paths.AssetsFolder, Paths.PackageDescription), typeof(TextAsset)) as TextAsset;

                if (halodiPackage == null)
                {
                    PackageConfigurationWindow packageNameWindow = ScriptableObject.CreateInstance(typeof(PackageConfigurationWindow)) as PackageConfigurationWindow;
                    packageNameWindow.ShowUtility();
                }
            }
 
            internal class AssemblyDefinition
            {
                public string name;
            }



            private static string CreateFolder(string parent, string name)
            {
                AssetDatabase.CreateFolder(parent, name);
                return Path.Combine(parent, name);
            }

            private static void CreateTextFile(object obj, string parentPath, string name)
            {
                CreateTextFile(JsonUtility.ToJson(obj, true), parentPath, name);
            }
            private static void CreateTextFile(string str, string parentPath, string name)
            {
                string asset = Path.Combine(parentPath, name);
                StreamWriter writer = new StreamWriter(asset, false);
                writer.Write(str);
                writer.Close();
                AssetDatabase.ImportAsset(asset, ImportAssetOptions.ForceUpdate);
            }

            private static void CreateAssemblyFolder(string parentFolder, string folderName, string packageName)
            {
                string name = folderName + "." + packageName;
                AssemblyDefinition def = new AssemblyDefinition();
                def.name = Paths.AssemblyDefinitionPrefix + name;


                string folder = CreateFolder(parentFolder, folderName);
                CreateTextFile(def, folder, def.name + Paths.AssemblyDefinitionExtension);

            }

            public class HalodiPackage : ScriptableObject
            {
                public string PackageName;
            }


            internal static void CreatePackage(PackageManifest manifest)
            {
                HalodiPackage halodiPackage = ScriptableObject.CreateInstance(typeof(HalodiPackage)) as HalodiPackage;
                halodiPackage.PackageName = manifest.name;

                CreateTextFile(halodiPackage, Paths.AssetsFolder, Paths.PackageDescription);

                string packageFolder = CreateFolder(Paths.AssetsFolder, manifest.name);


                CreateAssemblyFolder(packageFolder, Paths.EditorFolder, manifest.name);
                CreateAssemblyFolder(packageFolder, Paths.RuntimeFolder, manifest.name);

                string testFolder = CreateFolder(packageFolder, Paths.TestFolder);

                CreateAssemblyFolder(testFolder, Paths.EditorFolder, manifest.name + Paths.TestPackageNamePostfix);
                CreateAssemblyFolder(testFolder, Paths.RuntimeFolder, manifest.name + Paths.TestPackageNamePostfix);


                CreateTextFile(manifest, packageFolder, Paths.PackageManifest);
                CreateTextFile("", packageFolder, Paths.Readme);
                CreateTextFile("", packageFolder, Paths.License);
                CreateTextFile("", packageFolder, Paths.Changelog);

            }

        }

    }
}