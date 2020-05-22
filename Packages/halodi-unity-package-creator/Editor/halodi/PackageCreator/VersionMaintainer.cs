using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Halodi.PackageCreator
{

    internal class VersionMaintainer : AssetPostprocessor
    {
        private delegate void ListAction(PackageCollection collection, bool blocking);

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UpdateVersionInformation(false);
        }

        internal static void UpdateVersionInformation(bool blocking)
        {
            if (PackageGroupConfiguration.IsUseGroupVersion())
            {
                ListRequest(null, UpdateGroupVersion, blocking);
            }
            else
            {
                ListRequest(null, UpdateDependencies, blocking);
            }

        }


        private static void ListRequest(ListRequest listRequest, ListAction action, bool blocking)
        {
            if (listRequest == null)
            {
                listRequest = UnityEditor.PackageManager.Client.List(true, true);
            }

            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {
                    action(listRequest.Result, false);
                }
            }
            else
            {
                if (blocking)
                {
                    while (!listRequest.IsCompleted)
                    {
                        Thread.Sleep(10);
                    }
                    ListRequest(listRequest, action, true);
                }
                else
                {
                    EditorApplication.delayCall += () => ListRequest(listRequest, action, false);
                }
            }

        }


        private static void UpdateGroupVersion(PackageCollection collection, bool blocking)
        {

            if (PackageGroupConfiguration.IsUseGroupVersion())
            {
                string newVersion = PackageGroupConfiguration.GetGroupVersion();
                foreach (var package in collection)
                {
                    if (package.source == PackageSource.Embedded)
                    {
                        UpdatePackageVersion(package, newVersion);
                    }
                }
            }
            
            ListRequest(null, UpdateDependencies, blocking);
        }


        private static void UpdateDependencies(PackageCollection collection, bool blocking)
        {
            foreach (var package in collection)
            {
                if (package.source == PackageSource.Embedded)
                {
                    UpdateDependencyVersions(package, collection);
                }
            }
        }

        private static UnityEditor.PackageManager.PackageInfo GetPackage(string name, PackageCollection collection)
        {
            foreach (var package in collection)
            {
                if (package.name.Equals(name))
                {
                    return package;
                }
            }
            return null;
        }

        private static void UpdatePackageVersion(UnityEditor.PackageManager.PackageInfo package, string version)
        {
            if (package.version != version)
            {

                string manifest = Path.Combine(package.resolvedPath, "package.json");
                if (File.Exists(manifest))
                {
                    JObject jManifest = JObject.Parse(File.ReadAllText(manifest));
                    jManifest["version"] = version;

                    File.WriteAllText(manifest, jManifest.ToString());

                    AssetDatabase.Refresh();
                    Debug.Log("Updated group version for " + package.displayName);
                }
            }

        }

        private static void UpdateDependencyVersions(UnityEditor.PackageManager.PackageInfo package, PackageCollection collection)
        {
            string manifest = Path.Combine(package.resolvedPath, "package.json");
            if (File.Exists(manifest))
            {
                JObject jManifest = JObject.Parse(File.ReadAllText(manifest));

                bool changed = false;
                string changes = "";
                if (jManifest["dependencies"] != null)
                {
                    JObject deps = (JObject)jManifest["dependencies"];
                    foreach (KeyValuePair<string, JToken> dep in deps)
                    {

                        var PackageInfo = GetPackage(dep.Key, collection);

                        if (PackageInfo != null)
                        {
                            if (PackageInfo.version != dep.Value.ToString())
                            {
                                changes += "\t" + PackageInfo.name + "@" + dep.Value.ToString() + " => " + PackageInfo.version + Environment.NewLine;

                                ((JValue)dep.Value).Value = PackageInfo.version;
                                changed = true;
                            }
                        }
                    }
                }

                if (changed)
                {
                    Debug.Log("Updated dependency versions for " + package.displayName + Environment.NewLine + changes);
                    File.WriteAllText(manifest, jManifest.ToString());
                    AssetDatabase.Refresh();
                }
            }

        }
    }

}
