using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Halodi.PackageCreator
{
    internal class PublicationPackageView
    {
        internal bool publish;
        internal PackageManifest package;
        internal RegistrySelector RegistrySelector;
        internal string registry;

        public PublicationPackageView(PackageManifest package)
        {
            this.publish = false;
            this.package = package;
            this.RegistrySelector = new RegistrySelector();

            if (package.publishConfig != null && !string.IsNullOrEmpty(package.publishConfig.registry))
            {
                this.registry = package.publishConfig.registry;
            }
        }
    }


    internal class PublicationView : EditorWindow
    {

        private List<PublicationPackageView> PackagesToPublish = null;

        private bool publishAll;

        private Vector2 scrollPos;

        void OnEnable()
        {

        }

        void OnDisable()
        {
            PackagesToPublish = null;
            publishAll = false;
        }


        private void SetPackages(List<PackageManifest> packages)
        {
            PackagesToPublish = new List<PublicationPackageView>();
            foreach (var package in packages)
            {
                PackagesToPublish.Add(new PublicationPackageView(package));
            }
        }

        private void PackageGUI(PublicationPackageView packageView)
        {
            EditorGUI.BeginChangeCheck();
            packageView.publish = EditorGUILayout.BeginToggleGroup(packageView.package.displayName, packageView.publish);
            if (EditorGUI.EndChangeCheck())
            {
                if (!packageView.publish)
                {
                    publishAll = false;
                }
            }

            packageView.registry = packageView.RegistrySelector.SelectRegistry("\t", packageView.registry);
            EditorGUILayout.EndToggleGroup();
        }

        void OnGUI()
        {
            if (PackagesToPublish != null)
            {
                EditorGUILayout.LabelField("Publishing packages", EditorStyles.whiteLargeLabel);

                EditorGUILayout.Separator();

                EditorGUI.BeginChangeCheck();
                publishAll = EditorGUILayout.ToggleLeft("Publish all packages", publishAll);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (PublicationPackageView packageView in PackagesToPublish)
                    {
                        packageView.publish = publishAll;
                    }
                }


                EditorGUILayout.Separator();



                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                foreach (PublicationPackageView packageView in PackagesToPublish)
                {
                    PackageGUI(packageView);
                }
                EditorGUILayout.EndScrollView();




                EditorGUILayout.Separator();

                if (GUILayout.Button("Publish"))
                {
                    Publish();
                    Close();
                    GUIUtility.ExitGUI();

                }

                if (GUILayout.Button("Cancel"))
                {
                    Close();
                    GUIUtility.ExitGUI();
                }
            }
        }


        void Publish()
        {
            if (PackagesToPublish != null)
            {
                // Make sure the version information is updated before publishing
                VersionMaintainer.UpdateVersionInformation(true);

                int packagesToPublish = 0;
                foreach (var packageView in PackagesToPublish)
                {
                    if (packageView.publish)
                    {
                        packagesToPublish++;
                    }
                }

                if (packagesToPublish == 0)
                {
                    EditorUtility.DisplayDialog("Error", "No packages to publish selected.", "OK");
                    return;
                }



                EditorUtility.DisplayProgressBar("Publishing package", "Publishing packages", 0f);

                string result = "";
                int currentPackage = 0;
                bool failures = false;
                try
                {
                    foreach (var packageView in PackagesToPublish)
                    {
                        if (packageView.publish)
                        {
                            EditorUtility.DisplayProgressBar("Publishing package", "Publishing packages " + packageView.package.displayName + " to " + packageView.registry, (float)currentPackage / (float)packagesToPublish);

                            if (string.IsNullOrEmpty(packageView.registry))
                            {
                                failures = true;
                                result += "[Error] No registry set for " + packageView.package.displayName + Environment.NewLine;
                            }
                            else
                            {
                                try
                                {
                                    PublicationController.Publish(packageView.package, packageView.registry);
                                    result += "[Success] Publishing " + packageView.package.displayName + " succeeded." + Environment.NewLine;
                                }
                                catch (System.Exception e)
                                {
                                    failures = true;
                                    result += "[Error] Publishing " + packageView.package.displayName + " failed with error:" + Environment.NewLine + "\t" + e.Message + Environment.NewLine;
                                }
                            }

                            currentPackage++;
                        }
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
                string message;
                if (failures)
                {
                    message = "Published with errors." + Environment.NewLine + result;
                }
                else
                {
                    message = "Published all packages. " + Environment.NewLine + result;
                }
                EditorUtility.DisplayDialog("Publishing finished", message, "OK");


            }



        }



        public static void PublishPackages(List<PackageManifest> packages)
        {
            PublicationView publicationView = EditorWindow.GetWindow<PublicationView>(true, "Package Publishing", true);
            publicationView.SetPackages(packages);
        }

    }

}