using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Halodi.PackageCreator
{

    [InitializeOnLoad]
    internal class CreateGitKeep
    {
        static CreateGitKeep()
        {
            Create(Application.dataPath);
        }


        internal static void Create(string path)
        {
            string gitkeep = Path.Combine(path, ".gitkeep");
            if(!File.Exists(gitkeep))
            {
                File.AppendAllText(gitkeep, "This file forces git to check-in the required Assets/ directory, even when it is empty. Do not remove.");
                File.SetAttributes(gitkeep, FileAttributes.Hidden);
            }
        }
    }
}