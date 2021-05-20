using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{
    internal class HalodiEditPackageView : EditorWindow
    {
        [MenuItem("Assets/Edit Package")]
        private static void EditPackageMenu()
        {
            Debug.Log(Selection.activeObject);
            PackageManifest manifest = HalodiPackageCreatorController.GetPackageManifest(Selection.activeObject);
            if (manifest != null)
            {
                EditPackage(manifest);
            }

        }

        [MenuItem("Assets/Edit Package", true)]
        private static bool EditPackageMenuValidation()
        {
            PackageManifest manifest = HalodiPackageCreatorController.GetPackageManifest(Selection.activeObject);
            return manifest != null;
        }


        private ExtendedPackagePropertiesUI extendedUI;
        private PackageManifest manifest;

        void OnEnable()
        {
            extendedUI = new ExtendedPackagePropertiesUI();

        }

        void OnDisable()
        {
        }



        void OnGUI()
        {
            if (manifest != null)
            {
                EditorGUILayout.LabelField("Display name: " + manifest.displayName);

                extendedUI.Draw(manifest);


                if (GUILayout.Button("Apply"))
                {
                    OnClickUpdate();
                }

                if (GUILayout.Button("Cancel"))
                {
                    Close();
                    GUIUtility.ExitGUI();
                }

            }

        }

        private void OnClickUpdate()
        {
            extendedUI.Store(manifest);


        }

        internal static void EditPackage(PackageManifest package)
        {
            HalodiEditPackageView editView = EditorWindow.GetWindow<HalodiEditPackageView>(true, "Edit package", true);
            editView.manifest = package;

        }
    }
}