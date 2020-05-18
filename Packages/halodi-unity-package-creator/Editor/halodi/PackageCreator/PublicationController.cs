using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Halodi.PackageRegistry;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


namespace Halodi.PackageCreator
{
    internal class PublicationController
    {



        internal static void CopySamples(PackageManifest manifest)
        {
            string AssetSampleDirectory = HalodiPackageCreatorController.GetAssetsSampleDirectory(manifest);

            if (Directory.Exists(AssetSampleDirectory))
            {
                EmptySamplesDirectory(manifest);
                string SamplesDirectory = Path.Combine(HalodiPackageCreatorController.GetPackageDirectory(manifest), Paths.PackageSamplesFolder);
                AssetDatabaseUtilities.CopyDirectory(HalodiPackageCreatorController.GetAssetsSampleDirectory(manifest), SamplesDirectory, true);
            }

        }


        internal static void EmptySamplesDirectory(PackageManifest manifest)
        {
            DirectoryInfo SamplesDirectory = new DirectoryInfo(Path.Combine(HalodiPackageCreatorController.GetPackageDirectory(manifest), Paths.PackageSamplesFolder));
            if (SamplesDirectory.Exists)
            {
                SamplesDirectory.Delete(true);
            }



        }



        internal static void Publish(PackageManifest manifest, string registry)
        {
            CopySamples(manifest);

            try
            {
                manifest.OnAfterDeserialize();
                string PackageFolder = Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), manifest.package_name);

                NPM.Publish(PackageFolder, registry);
            }
            finally
            {
                EmptySamplesDirectory(manifest);
            }
        }

    }
}