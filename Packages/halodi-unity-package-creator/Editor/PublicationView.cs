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
            if(PackageToPublish.publishConfig == null)
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
                    EditorGUILayout.LabelField("Package registry: " + PackageToPublish.publishConfig.registry);
                    EditorGUILayout.LabelField("To change the package registry, edit package.json in a text editor.");
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

        void Publish()
        {
            if(PackageToPublish != null)
            {
                string registry = GetRegistry();
                EditorUtility.DisplayProgressBar("Publishing package", "Publishing package to " + registry, 0.25f);
                try
                {
                    Debug.Log("Publishing package to " + registry);
                
                    if(PublicationController.Login(publicationModel, GetRegistry()))
                    {
                        EditorUtility.DisplayProgressBar("Publishing package", "Publishing package to " + registry, 0.5f);
                        string output = PublicationController.Publish(publicationModel, PackageToPublish, GetRegistry());
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(output, 0, 0);

                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Login failed", "Cannot login to " + registry, "Close");
                    }

                }
                catch(System.Exception e)
                {
                    Debug.LogError(e);
                }

                EditorUtility.ClearProgressBar();

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