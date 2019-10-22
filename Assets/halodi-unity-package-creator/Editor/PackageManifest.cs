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

        public class  Dependency
        {
            public string name;
            public string version;

            public bool markDeleted = false;
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

        public Repositiory repository = new Repositiory();

        public string license = "UNLICENSED";

        public Author author = new Author();            

        public PublishConfig publishConfig = new PublishConfig();

        [NonSerialized]
        public List<Dependency> dependency_list = new List<Dependency>();

        public Dictionary<string, string> dependencies = new Dictionary<string, string>();

        public List<string> keywords = new List<string>();

        public void OnBeforeSerialize()
        {
            name = name_space + "." + package_name;

            dependencies.Clear();
            foreach(Dependency dep in dependency_list)
            {
                if(!dep.markDeleted)
                {
                    dependencies.Add(dep.name, dep.version);
                }
            }
                
        }

        public void OnAfterDeserialize()
        {
            foreach(KeyValuePair<string,string> entry in dependencies)
            {
                Dependency dep = new Dependency();
                dep.name = entry.Key;
                dep.version = entry.Value;
                dependency_list.Add(dep);
            }
        }
    }

    
}