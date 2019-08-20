using System.Collections.Generic;

namespace Halodi
{
    namespace Editor
    {
        
        public class PackageManifest
        {
            public string name = "ExamplePackage";
            public string version = "0.0.1";

            public string displayName = "My package";

            public string description = "Example package";

            public string unity = "2019.2";

            public List<string> keywords = new List<string>();
        }

    }
}