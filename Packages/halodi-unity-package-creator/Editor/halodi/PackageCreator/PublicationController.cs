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



        internal static bool Publish(PackageManifest manifest, string registry, out string error)
        {
            CopySamples(manifest);


            try
            {


                manifest.OnAfterDeserialize();
                string PackageFolder = Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), manifest.package_name);

                string folder = FileUtil.GetUniqueTempPathInProject();
                PackRequest request = UnityEditor.PackageManager.Client.Pack(PackageFolder, folder);
                while (!request.IsCompleted)
                {
                    Thread.Sleep(100);
                }

                if (request.Status == StatusCode.Success)
                {
                    PackOperationResult result = request.Result;
                    Debug.Log(result.tarballPath);

                    string data = PublicationManifest.Create(manifest, registry, result.tarballPath);

                    bool success = NPM.Publish(registry, manifest.name, data, out error);

                    File.Delete(result.tarballPath);
                    Directory.Delete(folder);

                    return success;
                }
                else
                {
                    if (request.Error != null)
                    {
                        error = request.Error.message;
                    }
                    else
                    {
                        error = "Cannot pack package";
                    }

                    return false;
                }
            }
            finally
            {
                EmptySamplesDirectory(manifest);
            }




            // using (System.Diagnostics.Process npm = new System.Diagnostics.Process())
            // {

            //     npm.StartInfo = new System.Diagnostics.ProcessStartInfo
            //     {
            //         FileName = model.NPMExecutable,
            //         Arguments = "publish --registry " + registry,
            //         UseShellExecute = false,
            //         RedirectStandardError = true,
            //         RedirectStandardOutput = true,
            //         WorkingDirectory = HalodiPackageCreatorController.GetPackageDirectory(manifest)
            //     };

            //     npm.Start();

            //     string outputPath = FileUtil.GetUniqueTempPathInProject();
            //     StreamWriter writer = new StreamWriter(outputPath, false);


            //     while (!npm.StandardError.EndOfStream)
            //     {
            //         writer.WriteLine(npm.StandardError.ReadLine());
            //     }
            //     while (!npm.StandardOutput.EndOfStream)
            //     {
            //         writer.WriteLine(npm.StandardOutput.ReadLine());
            //     }

            //     writer.Close();
            //     EmptySamplesDirectory(manifest);
            //     return outputPath;
            // }
        }

    }
}