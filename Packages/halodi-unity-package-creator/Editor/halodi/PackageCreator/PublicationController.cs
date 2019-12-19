using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Halodi.PackageCreator
{
    internal class PublicationController
    {
        private static readonly string EDITOR_PREFS_PREFIX = "com.halodi.halodi-unity-package-creator.";



        internal static PublicationModel LoadModel()
        {
            PublicationModel model = new PublicationModel();

            string DefaultNPMExecutable = "";

#if UNITY_EDITOR_WIN
            DefaultNPMExecutable = "C:\\Program Files\\nodejs\\npm.cmd";
#elif UNITY_EDITOR_LINUX
            DefaultNPMExecutable = "/usr/bin/npm";
#elif UNITY_EDITOR_OSX
            DefaultNPMExecutable = "/usr/bin/npm";
#endif



            model.NPMExecutable = EditorPrefs.GetString(EDITOR_PREFS_PREFIX + "npm-executable", DefaultNPMExecutable);
            model.RegisteryURL = EditorPrefs.GetString(EDITOR_PREFS_PREFIX + "npm-registry-url", PublicationModel.DEFAULT_REGISTRY);
            model.user = EditorPrefs.GetString(EDITOR_PREFS_PREFIX + "npm-registry-user", "");
            model.password = EditorPrefs.GetString(EDITOR_PREFS_PREFIX + "npm-registry-password", "");
            model.email = EditorPrefs.GetString(EDITOR_PREFS_PREFIX + "npm-registry-email", "");


            return model;
        }

        internal static void SaveModel(PublicationModel model)
        {
            EditorPrefs.SetString(EDITOR_PREFS_PREFIX + "npm-executable", model.NPMExecutable);
            EditorPrefs.SetString(EDITOR_PREFS_PREFIX + "npm-registry-url", model.RegisteryURL);
            EditorPrefs.SetString(EDITOR_PREFS_PREFIX + "npm-registry-user", model.user);
            EditorPrefs.SetString(EDITOR_PREFS_PREFIX + "npm-registry-password", model.password);
            EditorPrefs.SetString(EDITOR_PREFS_PREFIX + "npm-registry-email", model.email);
        }

        internal static bool ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        internal static bool ValidateUser(string name)
        {
            if(name.Trim().Length == 0)
            {
                return false;
            }

            return Regex.IsMatch(name, @"^[a-z0-9_\-]*$");

        }

        internal static bool IsLoggedIn(PublicationModel model, string registry)
        {
            using (System.Diagnostics.Process npm = new System.Diagnostics.Process())
            {
                npm.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = model.NPMExecutable,
                    Arguments = "whoami --registry " + registry,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };
                npm.Start();
                if(!npm.WaitForExit(2000))
                {
                    npm.Kill();
                    return false;
                }
                

                if(npm.ExitCode == 0)
                {
                    string user = npm.StandardOutput.ReadToEnd().Trim();
                    return user == model.user;
                }
                else
                {
                    return false;
                }
            }
        }

        internal static bool Login(PublicationModel model, string registry)
        {

            if(!ValidateEmail(model.email))
            {
                return false;
            }

            if(!ValidateUser(model.user))
            {
                return false;
            }

            if(model.password.Length == 0)
            {
                return false;
            }

            if(IsLoggedIn(model, registry))
            {
                return true;
            }

            using (System.Diagnostics.Process npm = new System.Diagnostics.Process())
            {

                npm.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = model.NPMExecutable,
                    Arguments = "login --registry " + registry,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };


                bool setUser = false;
                bool setPassword = false;
                
                npm.Start();

                StringBuilder sb = new StringBuilder();
                while(!npm.StandardOutput.EndOfStream)
                {
                    int read = npm.StandardOutput.Read();
                    char next = (char) read;

                    if(next == ':')
                    {
                        string field = sb.ToString().Trim();
                        if(!setUser)
                        {
                            if(field != "Username")
                            {
                                Debug.LogError("Cannot parse NPM output. Expected Username:, got " + field);
                                npm.Kill();
                                break;
                            }

                            npm.StandardInput.WriteLine(model.user);
                            setUser = true;
                        }
                        else if(!setPassword)
                        {
                            if(field != "Password")
                            {
                                Debug.LogError("Cannot parse NPM output. Expected password:, got " + field);
                                npm.Kill();
                                break;
                            }
                            npm.StandardInput.WriteLine(model.password);
                            setPassword = true;
                        }
                        else
                        {
                            if(field != "Email")
                            {
                                Debug.LogError("Cannot parse NPM output. Expected email:, got " + field);
                                npm.Kill();
                                break;
                            }
                            npm.StandardInput.WriteLine(model.email);

                            break;
                        }
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(next);
                    }
                } 

                if(!npm.WaitForExit(2000))
                {
                    npm.Kill();
                }
                
                if(npm.ExitCode == 0)
                {
                    // Need to sleep for a little bit, otherwise the token on the server will not be valid
                    System.Threading.Thread.Sleep(1000);
                    return IsLoggedIn(model, registry);
                }
                else
                {
                    string stderr = npm.StandardError.ReadToEnd();
                    Debug.LogError(stderr);
                    return false;
                }
                
                

            }
        }


        internal static string Publish(PublicationModel model, PackageManifest manifest, string registry)
        {

            using (System.Diagnostics.Process npm = new System.Diagnostics.Process())
            {

                npm.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = model.NPMExecutable,
                    Arguments = "publish --registry " + registry,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = HalodiPackageCreatorController.GetPackageDirectory(manifest)
                };

                npm.Start();

                string outputPath = FileUtil.GetUniqueTempPathInProject();
                StreamWriter writer = new StreamWriter(outputPath, false);

                
                while (!npm.StandardError.EndOfStream)
                {
                    writer.WriteLine(npm.StandardError.ReadLine());
                }
                while (!npm.StandardOutput.EndOfStream)
                {
                    writer.WriteLine(npm.StandardOutput.ReadLine());
                }

                writer.Close();

                return outputPath;
            }
        }

    }
}