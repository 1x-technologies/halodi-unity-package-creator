using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    internal class PublicationView : EditorWindow
    {
        private PublicationModel publicationModel = null;

        private PackageManifest PackageToPublish = null;


        void OnEnable()
        {
            publicationModel = PublicationController.LoadModel();
        }


        private string GetRegistry()
        {
            if(PackageToPublish.publishConfig != null)
            {
                return PackageToPublish.publishConfig.registry;
            }
            else
            {
                return publicationModel.RegisteryURL;
            }
        }

        void OnGUI()
        {
            if(publicationModel != null && PackageToPublish != null)
            {
                EditorGUILayout.LabelField("Publishing " + PackageToPublish.displayName, EditorStyles.whiteLargeLabel);

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                publicationModel.NPMExecutable = EditorGUILayout.TextField("npm executable: ", publicationModel.NPMExecutable);

                if (GUILayout.Button("Browse..."))
                {
                    string path = EditorUtility.OpenFilePanel("Select npm Executable", "", "");
                    if(path.Length != 0)
                    {
                        publicationModel.NPMExecutable = path;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                if(PackageToPublish.publishConfig == null)
                {
                    publicationModel.RegisteryURL = EditorGUILayout.TextField("Package registry: ", publicationModel.RegisteryURL);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Package registry: " + PackageToPublish.publishConfig.registry);
                    if(GUILayout.Button("Edit"))
                    {
                        Close();
                        EditRegistry();
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Separator();
                publicationModel.user = EditorGUILayout.TextField("Registry user: ", publicationModel.user);
                publicationModel.password = EditorGUILayout.PasswordField("Registry password: ", publicationModel.password);
                publicationModel.email = EditorGUILayout.TextField("Registry email: ", publicationModel.email);

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                if (GUILayout.Button("Publish"))
                {
                    Close();
                    Publish();
                    GUIUtility.ExitGUI();
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
            if(PackageToPublish != null)
            {
                EditorUtility.DisplayDialog("Edit registry", "A publishConfig section is set in package.json. You can change the registry in the text editor that will be opened.", "Ok");
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Path.Combine(HalodiPackageCreatorController.GetPackageDirectory(PackageToPublish), Paths.PackageManifest), 0, 0);
            }


        }

        void Publish()
        {
            if(PackageToPublish != null)
            {
                string registry = GetRegistry();
                EditorUtility.DisplayProgressBar("Publishing package", "Publishing package to " + registry, 0.25f);


                bool status;
                string error;
                try
                {
                    Debug.Log("Publishing package to " + registry);
                
                    EditorUtility.DisplayProgressBar("Publishing package", "Publishing package to " + registry, 0.5f);


                    status = PublicationController.Publish(publicationModel, PackageToPublish, GetRegistry(), out error);                

                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                if(status)
                {
                    EditorUtility.DisplayDialog("Success", "Uploaded " + PackageToPublish + System.Environment.NewLine + error, "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("Failure", "Cannot upload " + PackageToPublish + System.Environment.NewLine + error, "Ok");
                }

                

            }


            
        }

        void OnDisable()
        {
            if(publicationModel != null)
            {
                PublicationController.SaveModel(publicationModel);
            }
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