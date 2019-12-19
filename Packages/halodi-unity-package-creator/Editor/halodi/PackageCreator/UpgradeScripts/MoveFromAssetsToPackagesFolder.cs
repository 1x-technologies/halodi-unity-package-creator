using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    [InitializeOnLoad]
    internal class MoveFromAssetsToPackagesFolder
    {
        [Serializable]
        internal class HalodiPackage
        {
            public string PackageFolder = null;
        }

        static MoveFromAssetsToPackagesFolder()
        {
            string packageText = AssetDatabaseUtilities.ReadTextFile(Application.dataPath, "HalodiPackage.json");
            if(packageText != null)
            {
                HalodiPackage packageDescription = JsonUtility.FromJson<HalodiPackage>(packageText);
                
                string dir = Path.Combine(Application.dataPath, packageDescription.PackageFolder);
                string manifest = Path.Combine(dir, Paths.PackageManifest);

                if(File.Exists(manifest))
                {
                    Debug.Log("Found package in Assets/" + packageDescription.PackageFolder + ". Moving to Packages/.");

                    string target = Path.Combine(AssetDatabaseUtilities.GetProjectRoot(), Paths.PackagesFolder);
                    if(!Directory.Exists(target))
                    {
                        Directory.CreateDirectory(target);
                    }

                    string targetPackageFolder = Path.Combine(target, packageDescription.PackageFolder);
                    if(Directory.Exists(targetPackageFolder))
                    {
                        Debug.LogError("Found package " + packageDescription.PackageFolder + " in Assets/, but cannot move to Packages/ because a package with the same directory name already exists.");
                        return;
                    }
                    
                    Directory.Move(dir, targetPackageFolder);
                    
                }
                File.Delete(Path.Combine(Application.dataPath, "HalodiPackage.json"));                

                AssetDatabaseUtilities.UpdateAssetDatabase();
            }
        }

    }
}