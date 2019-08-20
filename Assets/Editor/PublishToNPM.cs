using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    public class PublishToNPM : EditorWindow
    {
        string registry = Configuration.registry;


        [MenuItem("Halodi/PublishToNPM")] //creates a new menu tab
        public static void StartPublishToNPM()
        {
            PublishToNPM publishToNPM = ScriptableObject.CreateInstance(typeof(PublishToNPM)) as PublishToNPM;
            publishToNPM.ShowUtility();
        }


        void OnGUI()
        {
            EditorGUILayout.LabelField("Publish to NPM");
            registry = EditorGUILayout.TextField("Registry: ", registry);



            if (GUILayout.Button("Publish"))
            {
                Publish();

            }
        }

        void Publish()
        {
            Close();
            GUIUtility.ExitGUI();
        }

    }

}