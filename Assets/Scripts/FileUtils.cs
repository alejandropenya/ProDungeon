using System.IO;
using System.Text;
using UnityEngine;

namespace Utils
{
    public static class FileUtils
    {
        public static void SaveScriptable<T>(T scriptableObject, string fileName, string folder = "")
            where T : ScriptableObject
        {
            var folderPath = Application.persistentDataPath + "/";

            if (!string.IsNullOrEmpty(folder))
            {
                folderPath += folder + "/";
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var json = JsonUtility.ToJson(scriptableObject);


            var filePath = string.Join(Path.DirectorySeparatorChar.ToString(), Application.persistentDataPath,
                Path.DirectorySeparatorChar.ToString(), folder, Path.DirectorySeparatorChar.ToString(),
                $"{fileName}.bin");
            var bytes = Encoding.UTF8.GetBytes(json);
            File.WriteAllBytes(filePath, bytes);
        }

        public static T LoadScriptable<T>() where T : ScriptableObject
        {
            var source = Application.persistentDataPath + $"/{typeof(T).Name}.bin";
            if (!File.Exists(source)) return default;
            var json = File.ReadAllText(source);

            var scriptable = ScriptableObject.CreateInstance(typeof(T));
            JsonUtility.FromJsonOverwrite(json, scriptable);
            return (T) scriptable;
        }
    }
}