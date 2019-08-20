using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Halodi.PackageCreator
{
    internal class PublicationModel
    {
        public static string DEFAULT_REGISTRY = "http://loki:4873";

        public string NPMExecutable;
        public string RegisteryURL = DEFAULT_REGISTRY;
    }
}