using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class PublicationManifest
    {

        private static string SHA512(string file)
        {
            using(FileStream stream = File.OpenRead(file))
            {
                var sha = new SHA512Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();   
            }
        }
        private static string SHA1(string file)
        {
            using(FileStream stream = File.OpenRead(file))
            {
                var sha = new SHA1Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();   
            }
        }

        public static string Size(string file)
        {
            return new FileInfo(file).Length.ToString();
        }

        public static string Data(string file)
        {
            Byte[] bytes = File.ReadAllBytes(file);
            return Convert.ToBase64String(bytes);
        }

        public static string Create(PackageManifest manifest, string registry, string file)
        {
            string tarballName = manifest.name + "-" + manifest.version + ".tgz";
            string tarballPath = manifest.name + "/-/" + tarballName;
            Uri registryUri = new Uri(registry);
            Uri tarballUri = new Uri(registryUri, tarballPath);

            JObject j = new JObject();
            

            j["_id"] = manifest.name;
            j["name"] = manifest.name;
            j["description"] = manifest.description;

            j["dist-tags"] = new JObject();
            j["dist-tags"]["latest"] = manifest.version;

            j["versions"] = new JObject();
            j["versions"][manifest.version] = JObject.Parse(JsonUtility.ToJson(manifest));
            j["versions"][manifest.version]["_id"] = manifest.name + "@" + manifest.version;
            j["versions"][manifest.version]["dist"] = new JObject();
            j["versions"][manifest.version]["dist"]["shasum"] = SHA1(file);
            j["versions"][manifest.version]["dist"]["integrity"] = SHA512(file);
            j["versions"][manifest.version]["dist"]["tarball"] = tarballUri.ToString();


            j["_attachments"] = new JObject();
            j["_attachments"][tarballName] = new JObject();
            j["_attachments"][tarballName]["content_type"] = "application/octet-stream";
            j["_attachments"][tarballName]["length"] = Size(file);
            j["_attachments"][tarballName]["data"] = Data(file);

            return j.ToString();

        }
    }
}

