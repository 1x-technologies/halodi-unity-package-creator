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
            EditorApplication.delayCall += () => EditorWindow.GetWindow<PublicationView>(true, "Package Publishing", true);  
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
                publicationModel.NPMExecutable = EditorGUILayout.TextField("NPM Executable: ", publicationModel.NPMExecutable);

                if (GUILayout.Button("Browse..."))
                {
                    string path = EditorUtility.OpenFilePanel("Select NPM Executable", "", "");
                    if(path.Length != 0)
                    {
                        publicationModel.NPMExecutable = path;
                    }
                }

                publicationModel.RegisteryURL = EditorGUILayout.TextField("Package registry: ", publicationModel.RegisteryURL);



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
            PublicationController.Publish(publicationModel);
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