using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    internal class PublishToNPM : EditorWindow
    {
        string registry = RegistryConfiguration.registry;


        [MenuItem("Halodi/PublishToNPM")] //creates a new menu tab
        internal static void StartPublishToNPM()
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