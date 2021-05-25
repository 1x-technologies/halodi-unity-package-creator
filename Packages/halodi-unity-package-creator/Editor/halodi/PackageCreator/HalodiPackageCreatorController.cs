using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class HalodiPackageCreatorController
    {

        public delegate void PackagesLoaded(List<PackageManifest> manifest);

        internal static void LoadPackages(PackagesLoaded callback, UnityEditor.PackageManager.PackageSource? source = null)
        {
            ListRequest request = UnityEditor.PackageManager.Client.List(true, false);
            LoadPackagesUpdate(request, callback, source);

        }

        private static void LoadPackagesUpdate(ListRequest request, PackagesLoaded callback, UnityEditor.PackageManager.PackageSource? source)
        {
            if (request.IsCompleted)
            {
                List<PackageManifest> packages = new List<PackageManifest>();
                foreach (var package in request.Result)
                {
                    try
                    {
                        if (source.HasValue)
                        {
                            if (package.source != source.Value)
                            {
                                continue;
                            }
                        }

                        PackageManifest manifest = new PackageManifest(package);
                        packages.Add(manifest);
                    }
                    catch
                    {
                        Debug.LogWarning("Cannot load manifest for package " + package.name);

                    }
                }

                callback(packages);
            }
            else
            {
                EditorApplication.delayCall += () => LoadPackagesUpdate(request, callback, source);
            }
        }



        internal static void EmbedPackageFromGit(PackageManifest manifest)
        {
            string targetDirectory = Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), manifest.package_name);

            if(File.Exists(targetDirectory))
            {
                Debug.LogWarning("Package directory " + targetDirectory + " already exists");
                return;
            }

            string gitCommand = "clone \"" + manifest.repository.url + "\"" + " " + "\"" + targetDirectory + "\"";
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "git";
                process.StartInfo.Arguments = gitCommand;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += StdOut;
                process.ErrorDataReceived += StdErr;


                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                
                EditorApplication.delayCall += () => EmbedPackageProgress(process, manifest);
            }
            catch (Exception e)
            {
                Debug.Log("Cannot find git executable: " + e.Message);
            }
        }

        private static void EmbedPackageProgress(System.Diagnostics.Process process, PackageManifest manifest)
        {
            if(process.HasExited)
            {
                EditorUtility.ClearProgressBar();
                Debug.Log("Git clone finished with exit code " + process.ExitCode);
                process.Dispose();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Cloning " + manifest.name, "Cloning " + manifest.name, 0.5f);
                EditorApplication.delayCall += () => EmbedPackageProgress(process, manifest);
            }
        }

        private static void StdErr(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.Data))
            {
                Debug.LogWarning(sender + " " + e.Data);
            }
        }

        private static void StdOut(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.Data))
            {
                Debug.Log(sender + " " + e.Data);
            }
        }


        /// <summary>
        /// Get the package manifest for this object, or null if not in a package
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static PackageManifest GetPackageManifest(UnityEngine.Object obj, bool onlyEmbedded)
        {
            if(obj == null)
            {
                return null;
            }
            
            string selectedPath = AssetDatabase.GetAssetPath(obj);
            if(String.IsNullOrEmpty(selectedPath))
            {
                return null;
            }
            var info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(selectedPath);

            if (info == null)
            {
                return null;
            }

            if (onlyEmbedded)
            {
                if (info.source != UnityEditor.PackageManager.PackageSource.Embedded)
                {
                    return null;
                }
            }

            try
            {
                return new PackageManifest(info);
            }
            catch
            {
                Debug.LogWarning("Cannot load manifest for " + info.name);
                return null;
            }


        }

        internal static TextAsset GetPackageManifestObject(PackageManifest manifest)
        {
            return manifest.asset;
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

        internal static string GetAssetsSampleDirectory(PackageManifest manifest)
        {
            return Path.Combine(GetAssetDirectory(manifest), Paths.AssetsSamplesFolder);
        }



        internal static void AddSample(PackageManifest manifest, PackageManifest.Sample sample)
        {

            string assetDirectory = GetAssetDirectory(manifest);
            string samplesDirectory = GetAssetsSampleDirectory(manifest);

            if (!sample.path.StartsWith(Paths.PackageSamplesFolder + "/"))
            {
                throw new System.Exception("Invalid sample directory");
            }


            Directory.CreateDirectory(samplesDirectory);

            string sampleFolderName = sample.path.Substring(Paths.PackageSamplesFolder.Length + 1);
            string sampleFolder = Path.Combine(samplesDirectory, sampleFolderName);

            Directory.CreateDirectory(sampleFolder);
            CreateGitKeep.Create(sampleFolder);



            JObject manifestJSON = JObject.Parse(GetPackageManifestObject(manifest).text);

            var samplesJSON = (JArray)manifestJSON["samples"];


            JObject next = new JObject(
                    new JProperty("displayName", sample.displayName),
                    new JProperty("description", sample.description),
                    new JProperty("path", sample.path));

            samplesJSON.Add(next);


            AssetDatabaseUtilities.CreateTextFile(manifestJSON.ToString(), GetPackageDirectory(manifest), Paths.PackageManifest);
            AssetDatabaseUtilities.UpdateAssetDatabase();
        }


    }
}
