# if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;
using static UnityEditor.AssetDatabase;
using System.IO;

namespace akira
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDirectories("_Project",
                "_Scripts>Controllers",
                "_Scripts>Editor",
                "_Scripts>Managers",
                "_Scripts>Scriptables",
                "_Scripts>Spawners",
                "_Scripts>Systems",
                "_Scripts>UI",
                "_Scripts>Units",
                "_Scripts>Utilities",
                "Animations",
                "Audio>Music",
                "Materials",
                "Prefabs>Props",
                "Prefabs>UI Prefabs",
                "Resources>Fonts",
                "Resources>Shaders",
                "Sprites>UI",
                "Scenes"
            );
            Refresh();
        }

        [MenuItem("Tools/Setup/Load New Manifest")]
        static async void LoadNewManifest()
        {
            await Packages.ReplacePackageFromGist("7b3e5fd1f6bd7d18fd23b382765b938b");
            Debug.Log("Loaded new manifest successfully!");
        }

        [MenuItem("Tools/Setup/Packages/New Input System")]
        static async void AddNewInputSystem()
        {
            bool success = await Packages.InstallUnityPackage("com.unity.inputsystem");
            if (success)
                Debug.Log("New Input System package installed successfully.");
        }

        [MenuItem("Tools/Setup/Packages/Post Processing")]
        static async void AddPostProcessing()
        {
            bool success = await Packages.InstallUnityPackage("com.unity.postprocessing");
            if (success)
                Debug.Log("Post Processing package installed successfully.");
        }

        [MenuItem("Tools/Setup/Packages/Cinemachine")]
        static async void AddCinemachine()
        {
            bool success = await Packages.InstallUnityPackage("com.unity.cinemachine");
            if (success)
                Debug.Log("Cinemachine package installed successfully.");
        }
    }
}
