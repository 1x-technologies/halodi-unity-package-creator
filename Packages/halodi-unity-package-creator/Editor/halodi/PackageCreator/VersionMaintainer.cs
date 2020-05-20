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
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UpdateDependencies();
        }

        public static void UpdateDependencies()
        {
            ListRequest request = UnityEditor.PackageManager.Client.List(true, true);

            while (!request.IsCompleted)
            {
                Thread.Sleep(10);
            }

            if (request.Status == StatusCode.Success)
            {
                PackageCollection collection = request.Result;

                foreach (var package in collection)
                {
                    if (package.source == PackageSource.Embedded)
                    {
                        UpdateDependencyVersions(package, collection);
                    }
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

        static void UpdateDependencyVersions(UnityEditor.PackageManager.PackageInfo package, PackageCollection collection)
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

                if(changed)
                {
                    Debug.Log("Updated dependency versions for " + package.displayName + Environment.NewLine + changes);
                    File.WriteAllText(manifest, jManifest.ToString());
                    AssetDatabase.Refresh();
                }
            }

        }
    }

}
