
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Halodi.PackageCreator
{

    [System.Serializable]
    internal class PackageGroupConfigurationDescription
    {
        public bool useGroupVersion;
        public string groupVersion;
    }

    internal class PackageGroupConfiguration
    {

        private const string CommonConfig = "package-group.json";


        internal static bool IsValidVersion(string version)
        {
            return HalodiNewPackageController.ValidateVersion(version);
        }

        private static string GetGroupConfigFile()
        {
            return Path.Combine(AssetDatabaseUtilities.GetRelativeToProjectRoot(Paths.PackagesFolder), CommonConfig);
        }

        private static bool HasPackageGroupConfig()
        {
            return File.Exists(GetGroupConfigFile());
        }

        internal static bool IsUseGroupVersion()
        {
            if (HasPackageGroupConfig())
            {
                return JsonUtility.FromJson<PackageGroupConfigurationDescription>(File.ReadAllText(GetGroupConfigFile())).useGroupVersion;
            }
            else
            {
                return false;
            }
        }

        internal static string GetGroupVersion()
        {
            if (HasPackageGroupConfig())
            {
                return JsonUtility.FromJson<PackageGroupConfigurationDescription>(File.ReadAllText(GetGroupConfigFile())).groupVersion;
            }

            throw new System.IO.IOException("Cannot find " + CommonConfig);
        }

        internal static void SetGroupVersion(string version)
        {
            if(!IsValidVersion(version))
            {
                throw new System.Exception("Invalid semantic version (major.minor.patch) format");
            }

            PackageGroupConfigurationDescription desc = new PackageGroupConfigurationDescription();
            desc.groupVersion = version;
            desc.useGroupVersion = true;

            File.WriteAllText(GetGroupConfigFile(), JsonUtility.ToJson(desc));

            VersionMaintainer.UpdateVersionInformation(true);
        }

        internal static void UnsetGroupVersion()
        {

            if(HasPackageGroupConfig())
            {
                PackageGroupConfigurationDescription desc = new PackageGroupConfigurationDescription();
                desc.groupVersion = GetGroupVersion();
                desc.useGroupVersion = false;
                File.WriteAllText(GetGroupConfigFile(), JsonUtility.ToJson(desc));
            }
            

        }
    }
}
