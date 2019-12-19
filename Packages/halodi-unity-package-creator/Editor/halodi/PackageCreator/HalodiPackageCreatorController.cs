using System.Collections.Generic;
using System.IO;
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


        internal static Object GetPackageManifestObject(PackageManifest manifest)
        {
         
            string packageDirectory = Path.Combine(Paths.PackagesFolder, manifest.name);
            string packageManifest = Path.Combine(packageDirectory, Paths.PackageManifest);
            return AssetDatabase.LoadAssetAtPath(packageManifest, typeof(Object));
        }

        internal static string GetPackageDirectory(PackageManifest manifest)
        {
            return manifest.filesystem_location; 
        }

    }
}
