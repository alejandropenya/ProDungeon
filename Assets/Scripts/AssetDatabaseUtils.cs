using System.Collections.Generic;
using Extensions;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    #if UNITY_EDITOR
    public static class AssetDatabaseUtils
    {
        public static string FindAssetWithExactName(string name, out string foundPath, string className = "")
        {
            var typeSearch = string.IsNullOrEmpty(className) ? $"{name}" : $"{name} t:{className}";
            var results = AssetDatabase.FindAssets(typeSearch)
                .Select(guid => new
                {
                    guid,
                    path = AssetDatabase.GUIDToAssetPath(guid),
                })
                .Select(path => new
                {
                    path.guid,
                    path.path,
                    asset = AssetDatabase.LoadAssetAtPath<Object>(path.path),
                })
                .Where(x => x.asset.name == name);

            results = results.Distinct();

            if (results.Count() > 1)
            {
                Debug.LogError($"{name} has {results.Count()} assets and you searched for it");
            }

            foundPath = results.FirstOrDefault().path;
            return results.FirstOrDefault().guid;
        }

        public static string FindAssetWithExactName(string name, string className = "")
        {
            var path = "";
            return FindAssetWithExactName(name, out path, className);
        }

        public static string FindAssetPathWithExactName(string name, string className = "")
        {
            FindAssetWithExactName(name, out var path, className);
            return path;
        }

        public static T LoadAssetWithName<T>(string name, string className) where T : Object
        {
            var typeSearch = string.IsNullOrEmpty(className) ? "" : $"t:{className}";
            var results = AssetDatabase.FindAssets(typeSearch)
                .Select(guid => new
                {
                    guid,
                    path = AssetDatabase.GUIDToAssetPath(guid),
                })
                .Select(path => new
                {
                    path.guid,
                    path.path,
                    asset = AssetDatabase.LoadAssetAtPath<T>(path.path),
                }).Where(x => x.asset.name == name);

            if (!results.Any()) return null;

            return results.FirstOrDefault().asset;
        }

        public static IEnumerable<T> LoadAllAssetsContainingName<T>(string name, string className) where T : Object
        {
            var typeSearch = string.IsNullOrEmpty(className) ? "" : $"t:{className}";
            var results = AssetDatabase.FindAssets(typeSearch)
                .Select(guid => new
                {
                    guid,
                    path = AssetDatabase.GUIDToAssetPath(guid),
                })
                .Select(path => new
                {
                    path.guid,
                    path.path,
                    asset = AssetDatabase.LoadAssetAtPath<T>(path.path),
                })
                .Where(x => x.asset.name.Contains(name));

            return results.Select(x => x.asset);
        }

        public static IEnumerable<string> FindPathOfAllAssetsContainingName<T>(string name) where T : Object
        {
            var typeSearch = string.IsNullOrEmpty(typeof(T).Name) ? "" : $"t:{typeof(T).Name}";
            return AssetDatabase.FindAssets(typeSearch)
                .Select(guid => new
                {
                    guid,
                    path = AssetDatabase.GUIDToAssetPath(guid),
                })
                .Select(x => x.path);
        }

        public static void CreateAsset(Object asset, string path)
        {
            AssetDatabase.CreateAsset(asset, path);
        }

        public static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
        }

        public static void SetDirty(Object asset)
        {
            EditorUtility.SetDirty(asset);
        }

        public static void AddObjectToAsset(Object objectToAdd, Object receivingObject)
        {
            AssetDatabase.AddObjectToAsset(objectToAdd, receivingObject);
        }

        public static IEnumerable<T> GetChildObjectsOfAsset<T>(string name, string fatherType = "") where T : Object
        {
            var guid = FindAssetWithExactName(name, fatherType);
            var allChilds = AssetDatabase
                .LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid))
                .Where(x => x.name != name)
                .Select(x => x as T)
                .Where(x => x);
            return allChilds.CloneList();
        }

        public static void SaveAsset<T>(UnityEngine.Object obj, string path,
            List<UnityEngine.Object> internalObjs = null)
            where T : UnityEngine.Object
        {
            if (!AssetDatabase.Contains(obj))
            {
                AssetDatabaseUtils.CreateAsset(obj, path);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                obj = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            AssetDatabase.SetMainObject(obj, path);

            UnityEngine.Object[] existingInternalObjects =
                AssetDatabase.LoadAllAssetsAtPath(path).Remove(obj).ToArray();
            if (internalObjs != null && internalObjs.Count > 0)
            {
                for (int j = 0; j < internalObjs.Count; j++)
                {
                    if (existingInternalObjects.Contains(internalObjs[j]))
                    {
                        existingInternalObjects = existingInternalObjects.Remove(internalObjs[j]).ToArray();
                    }
                    else
                    {
                        AssetDatabase.AddObjectToAsset(internalObjs[j], obj);
                    }
                }
            }

            for (int j = 0; j < existingInternalObjects.Length; j++)
            {
                AssetDatabase.RemoveObjectFromAsset(existingInternalObjects[j]);
            }

            AssetDatabase.ImportAsset(path);
        }
    }
    #endif
}