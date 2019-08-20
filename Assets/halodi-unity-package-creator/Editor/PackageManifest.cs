using System;
using System.Collections.Generic;

namespace Halodi.PackageCreator
{


    [System.Serializable]
    internal class PackageManifest
    {

        [System.Serializable]
        public class Repositiory
        {
            public string type = "git";
            public string url = "";
        }

        [System.Serializable]
        public class Author
        {
            public string name = "";
            public string email = "";
            public string url = "http://www.halodi.com";
        }

        [System.Serializable]
        public class PublishConfig
        {
            public string registry = PublicationModel.DEFAULT_REGISTRY;
        }

        [NonSerialized]
        public string name_space = "com.halodi";

        [NonSerialized]
        public string package_name = "example-package";
        
        public string name;
        public string version = "0.0.1";

        public string displayName = "My package";

        public string description = "Example package";

        public string unity = "2019.2";

        public Repositiory repositiory = new Repositiory();

        public string license = "UNLICENSED";

        public Author author = new Author();            

        public PublishConfig publishConfig = new PublishConfig();

        public List<string> keywords = new List<string>();
    }

    
}