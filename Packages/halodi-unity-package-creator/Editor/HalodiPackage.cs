using System;
using System.Collections.Generic;

namespace Halodi.PackageCreator
{
    [Serializable]
    internal class HalodiPackage
    {
        public string PackageNamespace;
        public string PackageName;

        public string PackageFolder;
    }

    [Serializable]
    internal class HalodiPackages
    {
        public int version = 1;
        public List<HalodiPackage> packages = new List<HalodiPackage>();
    }
}