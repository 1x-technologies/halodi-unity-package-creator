using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class AssetDatabaseUtilities
    {
        internal static string GetProjectRoot()
        {
            DirectoryInfo dataInfo = new DirectoryInfo(Application.dataPath);
            return dataInfo.Parent.ToString();
        }

        internal static string GetRelativeToProjectRoot(string path)
        {
            return Path.Combine(GetProjectRoot(), path);
        }

        internal static string ReadTextFile(string parentPath, string name)
        {
            string asset = Path.Combine(parentPath, name);

            if (!File.Exists(asset))
            {
                return null;
            }

            try
            {
                using (StreamReader reader = new StreamReader(asset))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        internal static void CreateJSONFile(object obj, string parentPath, string name)
        {
            string objStr = JsonUtility.ToJson(obj, true);
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
        }

        internal static void UpdateTextFile(string str, string parentPath, string name)
        {
            CreateTextFile(str, parentPath, name);
        }


        internal static string CreateFolder(string parent, string name)
        {

            string newFolder = Path.Combine(parent, name);
            Directory.CreateDirectory(newFolder);
            return newFolder;
        }

        internal static void UpdateAssetDatabase()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
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
            string name = packageName + "." + folderName + (testFolder ? ".Tests" : "");
            string folder = AssetDatabaseUtilities.CreateFolder(parentFolder, folderName);
            AssemblyDefinition def = new AssemblyDefinition();
            def.name = name;

            if (editor)
            {
                def.includePlatforms.Add("Editor");
            }


            if (references != null)
            {
                def.references.AddRange(references);
            }

            if (testFolder)
            {
                def.optionalUnityReferences.Add("TestAssemblies");
            }


            AssetDatabaseUtilities.CreateJSONFile(def, folder, def.name + Paths.AssemblyDefinitionExtension);

            string AssemblyDefinition = @"
using System.Reflection;

[assembly: AssemblyTitle(""" + def.name + @""")]
[assembly: AssemblyProduct(""" + packageName + @""")]
";

            AssetDatabaseUtilities.CreateTextFile(AssemblyDefinition, folder, "AssemblyInfo.cs");        
        

            return def;
        }

        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        internal static bool IsValidFolder(string path)
        {
            return Directory.Exists(path);
        }
    }
}