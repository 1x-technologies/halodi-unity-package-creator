using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{


    [System.Serializable]
    internal class PackageManifest
    {

        public PackageManifest()
        {
            
        }

        public PackageManifest(UnityEditor.PackageManager.PackageInfo info)
        {
            this.info = info;
            var location = Path.Combine(info.assetPath, Paths.PackageManifest);
            asset = AssetDatabase.LoadAssetAtPath<TextAsset>(location);

            if(asset == null)
            {
                throw new System.Exception("Cannot load asset at path " + location);
            }

            JsonUtility.FromJsonOverwrite(asset.text, this);
            OnAfterDeserialize();
            filesystem_location = info.resolvedPath;
        }

        [System.Serializable]
        public class PublishConfig
        {
            public string registry = "";
        }

        [System.Serializable]
        public class Sample
        {
            public string displayName = "";
            public string description = "";
            public string path = "";
        }

        [System.Serializable]
        public class Author
        {
            public string name = "";
            public string email = "";
            public string url = "";
        }

        [System.Serializable]
        public class Repository
        {
            public string type = "";
            public string url = "";
        }

        [NonSerialized]
        public UnityEditor.PackageManager.PackageInfo info;

        [NonSerialized]
        public TextAsset asset;

        public bool IsEmbedded => info.source == UnityEditor.PackageManager.PackageSource.Embedded;

        [NonSerialized]
        public string filesystem_location;

        [NonSerialized]
        public string name_space = "com.halodi";

        [NonSerialized]
        public string package_name = "";
        
        public string name;
        public string version = "0.0.1";

        public string displayName = "";

        public string description = "";

        public string license = "proprietary";

        public PublishConfig publishConfig = new PublishConfig();

        public List<Sample> samples = null;

        public Author author = new Author();

        public Repository repository = new Repository();


        public bool hideInEditor = false;

        public void OnBeforeSerialize()
        {
            name = name_space + "." + package_name;

            if(publishConfig != null)
            {
                if(string.IsNullOrEmpty(publishConfig.registry))
                {
                    publishConfig = null;
                }
                
            }
        }

        public void OnAfterDeserialize()
        {
            package_name = name.Substring(name.LastIndexOf(".") + 1);
            name_space = name.Substring(0, name.LastIndexOf("."));
        }
    }

    
}