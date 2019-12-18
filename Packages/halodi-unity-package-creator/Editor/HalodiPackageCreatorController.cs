using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    [InitializeOnLoad]
    internal class HalodiPackageCreatorController
    {
        static HalodiPackageCreatorController()
        {
            HalodiPackages packages = LoadPackages();
            
            if(packages == null)
            {
                HalodiPackages newPackages = new HalodiPackages();
                AssetDatabaseUtilities.CreateJSONFile(newPackages, Paths.PackagesFolder, Paths.HalodiPackagesDescription);
            }
        }


        internal static HalodiPackages LoadPackages()
        {
            string content = AssetDatabaseUtilities.ReadTextFile(Paths.PackagesFolder, Paths.HalodiPackagesDescription);

            if(content == null)
            {
                return null;
            }

            return JsonUtility.FromJson<HalodiPackages>(content);
        }


        internal static Object GetPackageManifest(HalodiPackage package)
        {
         
            string packageDirectory = Path.Combine(Paths.PackagesFolder, package.PackageNamespace + "." + package.PackageName);
            string packageManifest = Path.Combine(packageDirectory, Paths.PackageManifest);
            return AssetDatabase.LoadAssetAtPath(packageManifest, typeof(Object));
        }

    }
}
