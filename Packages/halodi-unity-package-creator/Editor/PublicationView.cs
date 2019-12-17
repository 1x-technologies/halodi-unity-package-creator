using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    internal class PublicationView : EditorWindow
    {
        private PublicationModel publicationModel = null;


        [MenuItem("Halodi/Publish Package")] //creates a new menu tab
        internal static void StartPublishToNPM()
        {
            if(PackageConfigurationController.PackageIsInitialized())
            {
                EditorApplication.delayCall += () => EditorWindow.GetWindow<PublicationView>(true, "Package Publishing", true);  
            }
        }

        void OnEnable()
        {
            publicationModel = PublicationController.LoadModel();
        }

        void OnGUI()
        {
            if(publicationModel != null)
            {
                EditorGUILayout.LabelField("Publish to Package Registry");
                publicationModel.NPMExecutable = EditorGUILayout.TextField("npm executable: ", publicationModel.NPMExecutable);

                if (GUILayout.Button("Browse..."))
                {
                    string path = EditorUtility.OpenFilePanel("Select npm Executable", "", "");
                    if(path.Length != 0)
                    {
                        publicationModel.NPMExecutable = path;
                    }
                }

                publicationModel.RegisteryURL = EditorGUILayout.TextField("Package registry: ", publicationModel.RegisteryURL);

                EditorGUILayout.LabelField("Create a npm user by running");
                EditorGUILayout.LabelField("`npm login --registry [package registry]`");
                EditorGUILayout.LabelField("before publishing");

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
            EditorUtility.DisplayProgressBar("Publishing package", "Publishing package to " + publicationModel.RegisteryURL, 0.1f);
            string output = PublicationController.Publish(publicationModel);
            EditorUtility.ClearProgressBar();

            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(output, 0, 0);
        }

        void OnDisable()
        {
            if(publicationModel != null)
            {
                PublicationController.SaveModel(publicationModel);
            }
        }

    }

}