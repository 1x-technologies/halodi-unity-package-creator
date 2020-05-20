using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    internal class PublicationView : EditorWindow
    {

        private PackageManifest PackageToPublish = null;
        private RegistrySelector RegistrySelector = null;

        private string registry;

        void OnEnable()
        {
            RegistrySelector = new RegistrySelector();
            registry = "";
        }

        private bool HasPublishConfigRegistry()
        {
            return PackageToPublish.publishConfig != null && !string.IsNullOrEmpty(PackageToPublish.publishConfig.registry);
        }

        private string GetRegistry()
        {
            if (HasPublishConfigRegistry())
            {
                return PackageToPublish.publishConfig.registry;
            }
            else
            {
                return registry;
            }
        }

        void OnGUI()
        {
            if (PackageToPublish != null)
            {
                EditorGUILayout.LabelField("Publishing " + PackageToPublish.displayName, EditorStyles.whiteLargeLabel);

                EditorGUILayout.Separator();


                if (HasPublishConfigRegistry())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Package registry: " + PackageToPublish.publishConfig.registry);
                    if (GUILayout.Button("Edit"))
                    {
                        Close();
                        EditRegistry();
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    registry = RegistrySelector.SelectRegistry("", registry);
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Publish"))
                {

                    if (string.IsNullOrEmpty(GetRegistry()))
                    {
                        EditorUtility.DisplayDialog("Failed", "No registry set for publication.", "OK");
                    }
                    else
                    {
                        Close();
                        Publish();
                        GUIUtility.ExitGUI();
                    }


                }

                if (GUILayout.Button("Cancel"))
                {
                    Close();
                    GUIUtility.ExitGUI();
                }
            }
        }

        void EditRegistry()
        {
            if (PackageToPublish != null)
            {
                EditorUtility.DisplayDialog("Edit registry", "A publishConfig section is set in package.json. You can change the registry in the text editor that will be opened.", "Ok");
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Path.Combine(HalodiPackageCreatorController.GetPackageDirectory(PackageToPublish), Paths.PackageManifest), 0, 0);
            }


        }

        void Publish()
        {
            if (PackageToPublish != null)
            {
                string registry = GetRegistry();


                EditorUtility.DisplayProgressBar("Publishing package", "Publishing package to " + registry, 0.25f);


                try
                {
                    Debug.Log("Publishing package to " + registry);

                    EditorUtility.DisplayProgressBar("Publishing package", "Publishing package to " + registry, 0.5f);


                    PublicationController.Publish(PackageToPublish, GetRegistry());
                    EditorUtility.DisplayDialog("Success", "Uploaded " + PackageToPublish.name + " to " + registry, "Ok");

                }
                catch (System.IO.IOException e)
                {
                    EditorUtility.DisplayDialog("Failure", "Cannot upload " + PackageToPublish.name +
                    System.Environment.NewLine + System.Environment.NewLine +
                    "Error: " + e.Message, "Ok");

                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

            }



        }

        void OnDisable()
        {
        }


        public static void PublishPackage(PackageManifest package)
        {
            EditorApplication.delayCall += () =>
            {
                PublicationView publicationView = EditorWindow.GetWindow<PublicationView>(true, "Package Publishing", true);
                publicationView.PackageToPublish = package;
            };
        }

    }

}