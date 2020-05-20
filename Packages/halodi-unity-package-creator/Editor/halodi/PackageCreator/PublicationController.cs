using System;
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
            try
            {
                CopySamples(manifest);

                manifest.OnAfterDeserialize();
                string PackageFolder = Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), manifest.package_name);

                NPM.Publish(PackageFolder, registry);
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
                CopySamples(manifest);
                manifest.OnAfterDeserialize();
                string packageFolder = Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), manifest.package_name);


                string folder = FileUtil.GetUniqueTempPathInProject();
                PackRequest request = UnityEditor.PackageManager.Client.Pack(packageFolder, folder);
                while (!request.IsCompleted)
                {
                    Thread.Sleep(100);
                }

                if (request.Status != StatusCode.Success)
                {
                    string message = "Cannot pack package.";
                    if (request.Error != null)
                    {
                        message += Environment.NewLine + request.Error.message;

                    }
                    EditorUtility.DisplayDialog("Failure", message, "OK");
                }


                PackOperationResult result = request.Result;
                EditorUtility.RevealInFinder(result.tarballPath);

            }
            finally
            {
                EmptySamplesDirectory(manifest);
            }
        }

    }
}