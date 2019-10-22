using System.Collections.Generic;
using System.IO;
using fastJSON;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class AssetDatabaseUtilities
    {
        internal static void CreateJSONFile(object obj, string parentPath, string name)
        {
            JSONParameters parameters = new JSONParameters();
            parameters.UseExtensions = false;
            string objStr = fastJSON.JSON.ToNiceJSON(obj, parameters);


            CreateTextFile(objStr, parentPath, name);
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

            public List<string> references = new List<string>();

            public List<string> includePlatforms = new List<string>();

            public List<string> excludePlatforms = new List<string>();

            public List<string> optionalUnityReferences = new List<string>();
        }

        internal static AssemblyDefinition CreateAssemblyFolder(string parentFolder, string folderName, string packageName, bool testFolder, bool editor, List<string> references)
        {
            string name = packageName + "." + folderName+ (testFolder?".Tests":"");
            string folder = AssetDatabaseUtilities.CreateFolder(parentFolder, folderName);
            AssemblyDefinition def = new AssemblyDefinition();
            def.name = name;

            if(editor)
            {
                def.includePlatforms.Add("Editor");
            }


            if(references != null)
            {   
                def.references.AddRange(references);
            }

            if(testFolder)
            {
                def.optionalUnityReferences.Add("TestAssemblies");
            }

            AssetDatabaseUtilities.CreateJSONFile(def, folder, def.name + Paths.AssemblyDefinitionExtension);

            return def;
        }
    }
}