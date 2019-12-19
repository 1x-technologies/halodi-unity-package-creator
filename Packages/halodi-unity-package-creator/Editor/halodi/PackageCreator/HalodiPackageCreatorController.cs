using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class HalodiPackageCreatorController
    {

        internal static List<PackageManifest> LoadPackages()
        {

            DirectoryInfo packageDirectory = new DirectoryInfo(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder));

            List<PackageManifest> packages = new List<PackageManifest>();
            foreach (DirectoryInfo directory in packageDirectory.EnumerateDirectories())
            {
                string manifestPath = Path.Combine(packageDirectory.ToString(), directory.Name);
                if(File.Exists(Path.Combine(manifestPath, Paths.PackageManifest)))
                {
                    string halodiPackage = AssetDatabaseUtilities.ReadTextFile(manifestPath, Paths.PackageManifest);

                    if(halodiPackage == null)
                    {
                        continue;
                    }
                    else
                    {
                        PackageManifest manifest = JsonUtility.FromJson<PackageManifest>(halodiPackage);
                        manifest.filesystem_location = manifestPath;
                        packages.Add(manifest);
                    }
                }
            }

            return packages;
        }


        internal static TextAsset GetPackageManifestObject(PackageManifest manifest)
        {
         
            string packageDirectory = Path.Combine(Paths.PackagesFolder, manifest.name);
            string packageManifest = Path.Combine(packageDirectory, Paths.PackageManifest);
            return (TextAsset) AssetDatabase.LoadAssetAtPath(packageManifest, typeof(TextAsset));
        }

        internal static string GetPackageDirectory(PackageManifest manifest)
        {
            return manifest.filesystem_location; 
        }

        private static string GetAssetDirectory(PackageManifest manifest)
        {
            string assetFolder = Application.dataPath;
            string packageFolderName = new DirectoryInfo(manifest.filesystem_location).Name;

            return Path.Combine(assetFolder, packageFolderName);
        }

        internal static string GetSampleDirectory(PackageManifest manifest)
        {
            return Path.Combine(GetAssetDirectory(manifest), Paths.AssetsSamplesFolder);
        }
        


        internal static void AddSample(PackageManifest manifest, PackageManifest.Sample sample)
        {

            string assetDirectory = GetAssetDirectory(manifest);
            string samplesDirectory = GetSampleDirectory(manifest);

            if(!sample.path.StartsWith(Paths.PackageSamplesFolder + "/"))
            {
                throw new System.Exception("Invalid sample directory");
            }


            Directory.CreateDirectory(samplesDirectory);

            string sampleFolderName = sample.path.Substring(Paths.PackageSamplesFolder.Length + 1);
            string sampleFolder = Path.Combine(samplesDirectory, sampleFolderName);

            Directory.CreateDirectory(sampleFolder);
            CreateGitKeep.Create(sampleFolder);
                
            var manifestJSON = SimpleJSON.JSON.Parse(GetPackageManifestObject(manifest).text);
            

            var samplesJSON = manifestJSON["samples"].AsArray;
            int next = samplesJSON.Count;
            samplesJSON[next]["displayName"] = sample.displayName;
            samplesJSON[next]["description"] = sample.description;
            samplesJSON[next]["path"] = sample.path;

            AssetDatabaseUtilities.CreateTextFile(manifestJSON.ToString(4), GetPackageDirectory(manifest), Paths.PackageManifest);
            AssetDatabaseUtilities.UpdateAssetDatabase();
        }

    }
}
