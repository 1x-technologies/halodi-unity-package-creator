using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


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
            TextAsset halodiPackage = AssetDatabase.LoadAssetAtPath(Path.Combine(Paths.AssetsFolder, Paths.PackageDescription), typeof(TextAsset)) as TextAsset;
            if(halodiPackage == null)
            {
                return null;
            }
            else
            {
                return JsonUtility.FromJson<HalodiPackage>(halodiPackage.text);    
            }
        }

        internal static bool PackageIsInitialized()
        {
            return LoadPackage() != null;
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

            string PackageFolder = Path.Combine(Paths.AssetsFolder, package.PackageFolder);
                
            TextAsset manifestAsset = AssetDatabase.LoadAssetAtPath(Path.Combine(PackageFolder, Paths.PackageManifest), typeof(TextAsset)) as TextAsset;
            if(manifestAsset == null)
            {
                return null;
            }

            PackageManifest manifest = JsonUtility.FromJson<PackageManifest>(manifestAsset.text);
            manifest.name_space = package.PackageNamespace;
            manifest.package_name = package.PackageName;

            return manifest;
        }


        private static void CreatePackage(PackageManifest manifest)
        {
            HalodiPackage halodiPackage = new HalodiPackage();
            halodiPackage.PackageName = manifest.package_name;
            halodiPackage.PackageNamespace = manifest.name_space;
            halodiPackage.PackageFolder = manifest.package_name;

            AssetDatabaseUtilities.CreateJSONFile(halodiPackage, Paths.AssetsFolder, Paths.PackageDescription);

            string packageFolder = AssetDatabaseUtilities.CreateFolder(Paths.AssetsFolder, halodiPackage.PackageFolder);


            AssetDatabaseUtilities.CreateAssemblyFolder(packageFolder, Paths.EditorFolder, manifest.name, false);
            AssetDatabaseUtilities.CreateAssemblyFolder(packageFolder, Paths.RuntimeFolder, manifest.name, false);

            string testFolder = AssetDatabaseUtilities.CreateFolder(packageFolder, Paths.TestFolder);


            AssetDatabaseUtilities.CreateAssemblyFolder(testFolder, Paths.EditorFolder, manifest.name, true);
            AssetDatabaseUtilities.CreateAssemblyFolder(testFolder, Paths.RuntimeFolder, manifest.name, true);


            AssetDatabaseUtilities.CreateJSONFile(manifest, packageFolder, Paths.PackageManifest);
            AssetDatabaseUtilities.CreateTextFile("", packageFolder, Paths.Readme);
            AssetDatabaseUtilities.CreateTextFile("", packageFolder, Paths.License);
            AssetDatabaseUtilities.CreateTextFile("", packageFolder, Paths.Changelog);
        }

        private static void UpdatePackage(HalodiPackage package, PackageManifest manifest)
        {
            package.PackageName = manifest.package_name;
            package.PackageNamespace = manifest.name_space;
            AssetDatabaseUtilities.UpdateJSONFile(package, Paths.AssetsFolder, Paths.PackageDescription);

            string packageFolder = Path.Combine(Paths.AssetsFolder, package.PackageFolder);

            if(!AssetDatabase.IsValidFolder(packageFolder))
            {
                throw new IOException("Package folder " + package.PackageFolder + " does not exist");
            }

            AssetDatabaseUtilities.UpdateJSONFile(manifest, packageFolder, Paths.PackageManifest);
            
        }

        internal static void UpdateOrCreatePackage(PackageManifest manifest)
        {
            manifest.name = manifest.name_space + "." + manifest.package_name;

            HalodiPackage package = LoadPackage();
            if(package == null)
            {
                CreatePackage(manifest);
            }
            else
            {
                UpdatePackage(package, manifest);
            }
        }

        internal static bool ValidateName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z][a-zA-Z0-9_\-\.]*$");
        }
        
    }
    
}