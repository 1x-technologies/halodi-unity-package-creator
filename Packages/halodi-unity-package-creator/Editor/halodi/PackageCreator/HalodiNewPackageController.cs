using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Halodi.PackageCreator.AssetDatabaseUtilities;
using System.Text;

namespace Halodi.PackageCreator
{


    internal class HalodiNewPackageController
    {

        private static string CreateReadme(PackageManifest manifest)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("# "); builder.AppendLine(manifest.displayName);

            builder.AppendLine();

            builder.AppendLine(manifest.description);

            return builder.ToString();
        }

        private static string CreateChangelog(PackageManifest manifest)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# ChangeLog"); 
            builder.AppendLine();
            builder.AppendLine();


            builder.Append("## "); builder.Append(manifest.version); builder.Append(" - "); builder.AppendLine(DateTime.Now.ToString("yyyy-MM-dd")); 
            builder.Append("- Package created");

            return builder.ToString();
        }

        private static string CreateLicense(PackageManifest manifest)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# License");
            builder.AppendLine();
            builder.Append("Copyright (C) "); builder.AppendLine(DateTime.Now.ToString("yyyy")); 
            builder.AppendLine();
            builder.Append(manifest.displayName); builder.AppendLine(" can not be copied and/or distributed without the express permission of the author(s).");
            return builder.ToString();
        }

        internal static void CreatePackage(PackageManifest manifest)
        {   
            manifest.OnBeforeSerialize();

            try
            {
                AssetDatabase.StartAssetEditing();
                string PackageFolderName = manifest.package_name;
                string packageFolder = AssetDatabaseUtilities.CreateFolder(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), PackageFolderName);


                AssemblyDefinition runtime = AssetDatabaseUtilities.CreateAssemblyFolder(packageFolder, Paths.RuntimeFolder, manifest.name, false, false, null);
                AssemblyDefinition editor = AssetDatabaseUtilities.CreateAssemblyFolder(packageFolder, Paths.EditorFolder, manifest.name, false, true, new List<string>{ runtime.name });

                string testFolder = AssetDatabaseUtilities.CreateFolder(packageFolder, Paths.TestFolder);


                AssemblyDefinition runtimeTests = AssetDatabaseUtilities.CreateAssemblyFolder(testFolder, Paths.RuntimeFolder, manifest.name, true, false, new List<string> { runtime.name });
                AssetDatabaseUtilities.CreateAssemblyFolder(testFolder, Paths.EditorFolder, manifest.name, true, true, new List<string> { runtime.name, editor.name });


                AssetDatabaseUtilities.CreateJSONFile(manifest, packageFolder, Paths.PackageManifest);
                AssetDatabaseUtilities.CreateTextFile(CreateReadme(manifest), packageFolder, Paths.Readme);
                AssetDatabaseUtilities.CreateTextFile(CreateLicense(manifest), packageFolder, Paths.License);
                AssetDatabaseUtilities.CreateTextFile(CreateChangelog(manifest), packageFolder, Paths.Changelog);

            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabaseUtilities.UpdateAssetDatabase();
            }

            
        }

        internal static bool ValidateVersion(string version)
        {
            if(Regex.IsMatch(version, @"^0+\.0+\.0+$"))
            {
                return false;
            }

            return Regex.IsMatch(version, @"^[0-9]+\.[0-9]+\.[0-9]+$");
        }

        internal static bool ValidateName(string name)
        {
            if(name.Trim().Length == 0)
            {
                return false;
            }

            return Regex.IsMatch(name, @"^[a-z][a-z0-9_\-]*$");
        }

        internal static bool ValidateFolderName(string name)
        {
            if(name.Trim().Length == 0)
            {
                return false;
            }

            return Regex.IsMatch(name, @"^[a-zA-Z][a-zA-Z0-9_\-\ ]*$");
        }


        internal static bool ValidateNameSpace(string name)
        {
            return Regex.IsMatch(name, @"^[a-z][a-z0-9_\-\.]*[a-z0-9]$");
        }

        internal static bool PackageExists(string name)
        {
            return AssetDatabaseUtilities.IsValidFolder(Path.Combine(Paths.PackagesFolder, name));
        }
        
    }
    
}