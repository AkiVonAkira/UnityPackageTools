#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

namespace akira
{
    public class AutoAssetPrefix : AssetPostprocessor
    {
        #region Prefix Definitions

        private static readonly PrefixFileTypePair[] FileTypePairs =
        {
            new("anim", "AC"),
            new("controller", "CTRL"),
            new("fbx", "FBX"),
            new("mat", "M"),
            new("mp3", "SFX"),
            new("ogg", "SFX"),
            new("png", "SPR"),
            new("prefab", "P"),
            new("scenetemplate", "SCENE"),
            new("shader", "SHADER"),
            new("terrainlayer", "TL"),
            new("unity", "SCENE"),
            new("wav", "SFX"),
        };

        private static readonly PrefixAssetTypePair[] AssetTypePairs =
        {
            new(typeof(AnimationClip), "AC"),
            new(typeof(AudioClip), "SFX"),
            new(typeof(GameObject), "FBX"),
            new(typeof(Material), "M"),
            new(typeof(RuntimeAnimatorController), "CTRL"),
            new(typeof(SceneAsset), "SCENE"),
            new(typeof(ScriptableObject), "SO"),
            new(typeof(Shader), "SHADER"),
            new(typeof(TerrainData), "TD"),
            new(typeof(TerrainLayer), "TL"),
            new(typeof(Texture2D), "SPR"),
        };
        #endregion

        private static readonly HashSet<string> LoggedErrors = new();

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            foreach (string assetPath in importedAssets)
            {
                if (!assetPath.StartsWith("Assets/_Project"))
                {
                    continue;
                }

                if (assetPath.EndsWith(".cs"))
                {
                    continue;
                }

                var assetType = GetAssetType(assetPath);
                if (assetType == null)
                {
                    continue;
                }

                var splitFilePath = assetPath.Split('/');
                var splitFileName = splitFilePath.Last().Split('.');
                var fileNameWithoutExtension = splitFileName.First();
                var fileExtension = splitFileName.Last();

                var newName = GetNewName(fileNameWithoutExtension, fileExtension, assetType);
                if (!string.IsNullOrEmpty(newName))
                {
                    AssetDatabase.RenameAsset(assetPath, newName);
                    AssetDatabase.Refresh();

                    // Update the main object name to match the new filename
                    var newAssetPath =
                        $"{string.Join("/", splitFilePath.Take(splitFilePath.Length - 1))}/{newName}";
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(newAssetPath);
                    if (asset == null) continue;
                    asset.name = newName.Split('/').Last().Split('.').First();
                    EditorUtility.SetDirty(asset);
                }
            }
        }

        private static Type GetAssetType(string assetPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset != null)
            {
                if (asset is ScriptableObject)
                {
                    return typeof(ScriptableObject);
                }

                // Ignore DefaultAsset type
                if (asset is DefaultAsset)
                {
                    return null;
                }

                return asset.GetType();
            }

            LogErrorOnce($"Failed to load asset at path: {assetPath}");
            return null;
        }

        private static string GetNewName(string fileNameWithoutExtension, string fileExtension, Type assetType)
        {
            foreach (var pair in FileTypePairs)
            {
                if (pair.FileType != fileExtension) continue;
                return fileNameWithoutExtension.Split('_').First() == pair.Prefix ? null : $"{pair.Prefix}_{fileNameWithoutExtension}.{fileExtension}";
            }

            foreach (var pair in AssetTypePairs)
            {
                if (pair.AssetType != assetType) continue;
                return fileNameWithoutExtension.Split('_').First() == pair.Prefix ? null : $"{pair.Prefix}_{fileNameWithoutExtension}.{fileExtension}";
            }

            LogErrorOnce($"Unknown file type: {fileExtension}");
            LogErrorOnce($"Unknown asset type for file: {fileNameWithoutExtension}.{fileExtension}");
            LogErrorOnce($"Unknown .asset file type: {assetType}");
            return null;
        }

        private static void LogErrorOnce(string message)
        {
            if (LoggedErrors.Contains(message)) return;
            Debug.LogError(message);
            LoggedErrors.Add(message);
        }
    }

    #region Editor

    internal struct PrefixAssetTypePair
    {
        public readonly Type AssetType;
        public readonly string Prefix;

        public PrefixAssetTypePair(Type assetType, string prefix)
        {
            AssetType = assetType;
            Prefix = prefix;
        }
    }

    internal struct PrefixFileTypePair
    {
        public readonly string FileType;
        public readonly string Prefix;

        public PrefixFileTypePair(string pFileType, string pPrefix)
        {
            FileType = pFileType;
            Prefix = pPrefix;
        }
    }
    #endregion
}
#endif
