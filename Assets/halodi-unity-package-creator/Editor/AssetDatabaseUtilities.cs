using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class AssetDatabaseUtilities
    {
        internal static void CreateJSONFile(object obj, string parentPath, string name)
        {
            CreateTextFile(JsonUtility.ToJson(obj, true), parentPath, name);
        }

        internal static void UpdateJSONFile(object obj, string parentPath, string name)
        {
            CreateJSONFile(obj, parentPath, name);
        }

        internal static void CreateTextFile(string str, string parentPath, string name)
        {
            string asset = Path.Combine(parentPath, name);
            StreamWriter writer = new StreamWriter(asset, false);
            writer.Write(str);
            writer.Close();
            AssetDatabase.ImportAsset(asset, ImportAssetOptions.ForceUpdate);
        }

        internal static void UpdateTextFile(string str, string parentPath, string name)
        {
            CreateTextFile(str, parentPath, name);
        }

        
        internal static string CreateFolder(string parent, string name)
        {
            AssetDatabase.CreateFolder(parent, name);
            return Path.Combine(parent, name);
        }

        internal class AssemblyDefinition
        {
            public string name;
        }



        internal static void CreateAssemblyFolder(string parentFolder, string folderName, string packageName, bool testFolder)
        {
            string name = packageName + "." + folderName+ (testFolder?".Tests":"");
            AssemblyDefinition def = new AssemblyDefinition();
            def.name = name;

            string folder = AssetDatabaseUtilities.CreateFolder(parentFolder, folderName);
            AssetDatabaseUtilities.CreateJSONFile(def, folder, def.name + Paths.AssemblyDefinitionExtension);
        }
    }
}