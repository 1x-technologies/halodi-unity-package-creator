using System;
using System.IO;
using Halodi.PackageRegistry.Core;
using Halodi.PackageRegistry.NPM;
using UnityEditor;


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
            try
            {
                CopySamples(manifest);

                manifest.OnAfterDeserialize();
                string PackageFolder = Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), manifest.package_name);

                NPMPublish.Publish(PackageFolder, registry);
            }
            finally
            {
                EmptySamplesDirectory(manifest);
            }
        }


        internal static void Pack(PackageManifest manifest)
        {
            try
            {
                // Make sure the version information is updated before publishing
                VersionMaintainer.UpdateVersionInformation(true);

                CopySamples(manifest);
                manifest.OnAfterDeserialize();
                string packageFolder = Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), manifest.package_name);


                string folder = FileUtil.GetUniqueTempPathInProject();


                try
                {
                    string tarballPath = PackageTarball.Create(packageFolder, folder);
                    EditorUtility.RevealInFinder(tarballPath);

                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Failure", e.Message, "OK");
                }


                

            }
            finally
            {
                EmptySamplesDirectory(manifest);
            }
        }

    }
}