using System.Text.RegularExpressions;
using Halodi.PackageRegistry.Core;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    internal class RegistrySelector
    {
        private const string slashReplace = "\u200A\u2215\u200A";

        private string[] registries;

        private int selectedRegistry;
        
        public RegistrySelector()
        {
            string[] unescapedRegistries = new CredentialManager().Registries;
            this.registries = new string[unescapedRegistries.Length];

            for(int i = 0; i < unescapedRegistries.Length; i++)
            {
                this.registries[i] = unescapedRegistries[i].Replace("/", slashReplace);
            }
        }

        internal string SelectRegistry(string prefix, string currentValue)
        {
            string newValue = EditorGUILayout.TextField(prefix + "registry: ", currentValue);

            EditorGUILayout.BeginHorizontal();
            selectedRegistry = EditorGUILayout.Popup("\t", selectedRegistry, registries);
            if(GUILayout.Button("Select"))
            {
                newValue = registries[selectedRegistry].Replace(slashReplace, "/");
            }
            EditorGUILayout.EndHorizontal();

            return newValue;

        }
    }
}