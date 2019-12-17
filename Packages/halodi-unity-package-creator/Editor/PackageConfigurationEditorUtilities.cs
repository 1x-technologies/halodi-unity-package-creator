using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    [InitializeOnLoad]
    internal class PackageConfigurationEditorUtilties
    {
        static PackageConfigurationEditorUtilties()
        {
            if (!PackageConfigurationController.PackageIsInitialized())
            {
                PackageConfigurationView.ShowWindow();
            }
        }

        [MenuItem("Halodi/Package Configuration")] //creates a new menu tab
        internal static void EditPackageConfiguration()
        {
            PackageConfigurationView.ShowWindow();
        }
    }

}
