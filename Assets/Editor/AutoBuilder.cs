/* 
AutoBuilder.cs
Automatically changes the target platform and creates a build.
 
Installation
Place in an Editor folder.
 
Usage
Go to File > AutoBuilder and select a platform. These methods can also be run from the Unity command line using -executeMethod AutoBuilder.MethodName.
 */
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class AutoBuilder
{
    static string GetProjectName()
    {
    string[] s = Application.dataPath.Split('/');
    return s[s.Length - 2];
    }

    private static string ProjectName = GetProjectName();
	//  Or you can set static name manually


    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

	// Check ENV       
    static bool EnvironmentVariablesMissing(string[] envvars)
    {
        string value;
        bool missing = false;
	Console.Write("Current Environment Variables: ");
        foreach (string envvar in envvars)
        {
            value = Environment.GetEnvironmentVariable(envvar);
	    Console.WriteLine(value);
            if (value == null)
            {
                Console.Write("BUILD ERROR: Required Environment Variable is not set: ");
                Console.WriteLine(envvar);
                missing = false;
            }
        }

        return missing;
    }

    [MenuItem("File/AutoBuilder/Android")]
    private static void PerformAndroidBuild()
    {
    string[] envvars = new string[]
        {
          "APP_ID", "ANDROID_KEYSTORE_NAME", "ANDROID_KEYSTORE_PASSWORD", "ANDROID_KEYALIAS_NAME", "ANDROID_KEYALIAS_PASSWORD"
        };
        if (EnvironmentVariablesMissing(envvars))
        {
            Environment.ExitCode = -1;
            return; // note, we cannot use Environment.Exit(-1) - the buildprocess will just hang afterwards
        }

        //Available Playersettings: https://docs.unity3d.com/ScriptReference/PlayerSettings.Android.html

        //set settings from environment variables
        //PlayerSettings.Android.useCustomKeystore = false;
      	//PlayerSettings.applicationIdentifier = Environment.GetEnvironmentVariable("APP_ID");
        //PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_NAME");
        //PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASSWORD");
        //PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_NAME");
        //PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASSWORD");
	
	//set minimum Android SDK version
	//PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

	//set the internal apk version to the current unix timestamp, so this increases with every build
        //PlayerSettings.Android.bundleVersionCode = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; 

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        if (!Directory.Exists("./build/Android/"))
            Directory.CreateDirectory("./build/Android/");
        BuildReport report = BuildPipeline.BuildPlayer(GetScenePaths(), "./build/Android/" + ProjectName + ".apk", BuildTarget.Android, BuildOptions.None);
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            EditorApplication.Exit(0);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
            EditorApplication.Exit(1);
        }
    }
}
