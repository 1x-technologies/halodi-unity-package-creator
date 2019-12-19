using System;
using System.Collections.Generic;
using UnityEngine;
using fastJSON;
namespace Halodi.PackageCreator
{


    [System.Serializable]
    internal class PackageManifest
    {
        [System.Serializable]
        public class PublishConfig
        {
            public string registry = PublicationModel.DEFAULT_REGISTRY;
        }

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

        public PublishConfig publishConfig = new PublishConfig();


        public void OnBeforeSerialize()
        {
            name = name_space + "." + package_name;
        }

        public void OnAfterDeserialize()
        {
        }
    }

    
}