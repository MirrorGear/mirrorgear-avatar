using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Collections;

public static class DependenciesWindowInstaller
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        Events.registeredPackages += OnPostProcess;
    }

    private static void OnPostProcess(PackageRegistrationEventArgs args)
    {
        Events.registeredPackages -= OnPostProcess;

        foreach (var packageInfo in args.added)
        {
            if (packageInfo.name.Equals("com.seerslab.mirror_gear.avatar"))
            {
                InstallDependenciesWindow window = (InstallDependenciesWindow)EditorWindow.GetWindow(typeof(InstallDependenciesWindow));
                window.Show();
                break;
            }
        }
    }
}

public class InstallDependenciesWindow : EditorWindow
{
    void OnGUI()
    {
        GUILayout.Label("Welcome to MirrorGear Avatar!");

        if (GUILayout.Button("Install Dependencies"))
        {
            // var request = Client.Add("https://github.com/vrm-c/UniVRM.git?path=/Assets/VRMShaders#v0.107.2");
            // while (!request.IsCompleted) { }
            // request = Client.Add("https://github.com/vrm-c/UniVRM.git?path=/Assets/UniGLTF#v0.107.2");
            // while (!request.IsCompleted) { }
            // request = Client.Add("https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM#v0.107.2");
            // while (!request.IsCompleted) { }


            // request = Client.Add("https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM10#v0.107.2");
            // while (!request.IsCompleted) { }

            // PackagesController.AddPackage("VRMShaders", "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRMShaders#v0.107.2");
            // PackagesController.AddPackage("UniGLTF", "https://github.com/vrm-c/UniVRM.git?path=/Assets/UniGLTF#v0.107.2");
            // PackagesController.AddPackage("VRM", "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM#v0.107.2");
            // PackagesController.AddPackage("VRM-1.0", "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM10#v0.107.2");

            PackagesController.InstallModules();
        }
    }
}

public static class PackagesController
{
    private const string PROGRESS_BAR_TITLE = "MirrorGearAvatar";

    public struct ModuleInfo
    {
        public string name;
        public string gitUrl;
    }

    public static readonly ModuleInfo[] Modules = {
        new ModuleInfo
        {
            name = "VRMShaders",
            gitUrl = "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRMShaders#v0.107.2"
        },
        new ModuleInfo
        {
            name = "UniGLTF",
            gitUrl = "https://github.com/vrm-c/UniVRM.git?path=/Assets/UniGLTF#v0.107.2",
        },
        new ModuleInfo
        {
            name = "VRM",
            gitUrl = "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM#v0.107.2"
        }
    };

    public static void InstallModules()
    {
        EditorUtility.DisplayProgressBar(PROGRESS_BAR_TITLE, "Installing modules...", 0);

        var installedModuleCount = 0f;

        foreach(ModuleInfo module in Modules)
        {   
            var progress = installedModuleCount++ / Modules.Length;
            EditorUtility.DisplayProgressBar(PROGRESS_BAR_TITLE, $"Installing module {module.name}", progress);
            AddModuleRequest(module.gitUrl);
        }
        
        EditorUtility.DisplayProgressBar(PROGRESS_BAR_TITLE, "All modules are installed.", 1);
        EditorUtility.ClearProgressBar();
    }

    public static void AddModuleRequest(string url)
    {
        AddRequest addRequest = Client.Add(url);
        while (!addRequest.IsCompleted) {}

        if (addRequest.Error != null)
        {
            AssetDatabase.Refresh();
            Debug.Log("Error: " + addRequest.Error.message);
        }
    }
}
