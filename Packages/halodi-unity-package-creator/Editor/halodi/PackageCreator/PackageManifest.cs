using System;
using System.Collections.Generic;
using UnityEngine;

namespace Halodi.PackageCreator
{


    [System.Serializable]
    internal class PackageManifest
    {
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

        public List<Sample> samples;

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