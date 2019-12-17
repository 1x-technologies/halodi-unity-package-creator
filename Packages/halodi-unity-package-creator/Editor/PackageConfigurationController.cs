using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Halodi.PackageCreator.AssetDatabaseUtilities;
using fastJSON;

namespace Halodi.PackageCreator
{



    internal class HalodiPackage
    {
        public string PackageNamespace;
        public string PackageName;

        public string PackageFolder;

    }

    internal class PackageConfigurationController
    {
        internal static HalodiPackage LoadPackage()
        {
            string halodiPackage = AssetDatabaseUtilities.ReadTextFile(Paths.PackagesFolder, Paths.PackageDescription);

            if(halodiPackage == null)
            {
                return null;
            }
            else
            {

                return JSON.ToObject<HalodiPackage>(halodiPackage);    
            }
        }

        internal static bool PackageIsInitialized()
        {
            HalodiPackage package = LoadPackage();           
            return package != null && !string.IsNullOrEmpty(package.PackageFolder);
        }

        internal static UnityEngine.Object GetPackageManifestObject()
        {
            PackageManifest manifest = LoadManifest();
            if(manifest == null)
            {
                return null;
            }

            string packageDirectory = Path.Combine(Paths.PackagesFolder, manifest.name);
            string packageManifest = Path.Combine(packageDirectory, Paths.PackageManifest);
            return AssetDatabase.LoadAssetAtPath(packageManifest, typeof(TextAsset));
        }

        internal static string PackageFolderOnDisk()
        {
            HalodiPackage package = LoadPackage();
            if(package == null)
            {
                return null;
            }
            return Path.Combine(Application.dataPath, package.PackageFolder);
        }

        internal static PackageManifest LoadManifest()
        {
            HalodiPackage package = LoadPackage();
            if(package == null)
            {
                return null;
            }

            if(package.PackageFolder == null)
            {
                return null;
            }

            string PackageFolder = Path.Combine(Paths.PackagesFolder, package.PackageFolder);
                
            string manifestAsset = AssetDatabaseUtilities.ReadTextFile(PackageFolder, Paths.PackageManifest);
            if(manifestAsset == null)
            {
                return null;
            }

            PackageManifest manifest = JSON.ToObject<PackageManifest>(manifestAsset);
            manifest.name_space = package.PackageNamespace;
            manifest.package_name = package.PackageName;
            manifest.OnAfterDeserialize();

            return manifest;
        }


        private static void CreatePackage(PackageManifest manifest)
        {
            HalodiPackage halodiPackage = new HalodiPackage();
            halodiPackage.PackageName = manifest.package_name;
            halodiPackage.PackageNamespace = manifest.name_space;
            halodiPackage.PackageFolder = manifest.package_name;

            AssetDatabaseUtilities.CreateJSONFile(halodiPackage, Paths.PackagesFolder, Paths.PackageDescription);

            string packageFolder = AssetDatabaseUtilities.CreateFolder(Paths.PackagesFolder, halodiPackage.PackageFolder);


            AssemblyDefinition runtime = AssetDatabaseUtilities.CreateAssemblyFolder(packageFolder, Paths.RuntimeFolder, manifest.name, false, false, null);
            AssemblyDefinition editor = AssetDatabaseUtilities.CreateAssemblyFolder(packageFolder, Paths.EditorFolder, manifest.name, false, true, new List<string>{ runtime.name });

            string testFolder = AssetDatabaseUtilities.CreateFolder(packageFolder, Paths.TestFolder);


            AssemblyDefinition runtimeTests = AssetDatabaseUtilities.CreateAssemblyFolder(testFolder, Paths.RuntimeFolder, manifest.name, true, false, new List<string> { runtime.name });
            AssetDatabaseUtilities.CreateAssemblyFolder(testFolder, Paths.EditorFolder, manifest.name, true, true, new List<string> { runtime.name, editor.name });

            AssetDatabaseUtilities.CreateJSONFile(manifest, packageFolder, Paths.PackageManifest);
            AssetDatabaseUtilities.CreateTextFile("", packageFolder, Paths.Readme);
            AssetDatabaseUtilities.CreateTextFile("", packageFolder, Paths.License);
            AssetDatabaseUtilities.CreateTextFile("", packageFolder, Paths.Changelog);

            AssetDatabaseUtilities.UpdateAssetDatabase();
        }

        private static void UpdatePackage(HalodiPackage package, PackageManifest manifest)
        {
            package.PackageName = manifest.package_name;
            package.PackageNamespace = manifest.name_space;
            AssetDatabaseUtilities.UpdateJSONFile(package, Paths.PackagesFolder, Paths.PackageDescription);

            string packageFolder = Path.Combine(Paths.PackagesFolder, package.PackageFolder);

            if(!AssetDatabaseUtilities.IsValidFolder(packageFolder))
            {
                throw new IOException("Package folder " + package.PackageFolder + " does not exist");
            }

            AssetDatabaseUtilities.UpdateJSONFile(manifest, packageFolder, Paths.PackageManifest);
            AssetDatabaseUtilities.UpdateAssetDatabase();
        }

        internal static void UpdateOrCreatePackage(PackageManifest manifest)
        {
            manifest.OnBeforeSerialize();
            

            if(!PackageIsInitialized())
            {
                CreatePackage(manifest);
            }
            else
            {
                UpdatePackage(LoadPackage(), manifest);
            }
        }

        internal static bool ValidateName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z][a-zA-Z0-9_\-\.]*$");
        }
        
    }
    
}