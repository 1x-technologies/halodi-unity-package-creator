using System;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Halodi.PackageCreator
{
    internal class ExtendedPackagePropertiesUI
    {

        internal PackageManifest manifest;
        string[] licenseList = null;

        int licenseIndex = 0;

        RegistrySelector registrySelector = null;

        internal ExtendedPackagePropertiesUI()
        {
            licenseList = SPDXLicenseList.Load().ToStringArray();
            registrySelector = new RegistrySelector();
        }

        public void Draw(PackageManifest manifest)
        {

            manifest.author.name = EditorGUILayout.TextField("Author organization: ", manifest.author.name);
            manifest.author.email = EditorGUILayout.TextField("Author email: ", manifest.author.email);
            manifest.author.url = EditorGUILayout.TextField("Author website: ", manifest.author.url);

            licenseIndex = EditorGUILayout.Popup("License: ", licenseIndex, licenseList);

            manifest.repository.url = EditorGUILayout.TextField("GIT repository: ", manifest.repository.url);

            EditorGUILayout.LabelField("Publication configuration");
            manifest.publishConfig.registry = registrySelector.SelectRegistry("\t", manifest.publishConfig.registry);


            // Set fields in manifest
            manifest.repository.type = "git";
            manifest.license = licenseList[licenseIndex];
        }


        internal void Store(PackageManifest manifest)
        {
            JObject manifestJSON = JObject.Parse(HalodiPackageCreatorController.GetPackageManifestObject(manifest).text);

            JObject author = new JObject(
                    new JProperty("name", manifest.author.name),
                    new JProperty("email", manifest.author.email),
                    new JProperty("url", manifest.author.url));

            manifestJSON["author"] = author;


            manifestJSON["license"] = manifest.license;

            JObject publicationConfig = new JObject(
                new JProperty("registry", manifest.publishConfig.registry)
            );

            manifestJSON["publishConfig"] = publicationConfig;


            JObject repo = new JObject(
                new JProperty("type", manifest.repository.type),
                new JProperty("url", manifest.repository.url)
            );
            manifestJSON["repository"] = repo;



            AssetDatabaseUtilities.CreateTextFile(manifestJSON.ToString(), HalodiPackageCreatorController.GetPackageDirectory(manifest), Paths.PackageManifest);
            AssetDatabaseUtilities.UpdateAssetDatabase();


        }
    }
}