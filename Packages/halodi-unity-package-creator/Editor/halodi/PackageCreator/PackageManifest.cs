using System;
using System.Collections.Generic;

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

        [System.Serializable]
        public class Sample
        {
            public string displayName;
            public string description;
            public string path;
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
        }

        public void OnAfterDeserialize()
        {
        }
    }

    
}