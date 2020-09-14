using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq.Expressions;

class MainPlayer
{
    [MenuItem("Assets/Show Asset Ids")]
    static void MenuShowIds()
    {
        var stringBuilder = new StringBuilder();
        string guid;
        long file;

        var dirs = Directory.GetFiles(@"D:\Development\_Assets\Mir2Conquera\Mir2\Assets\_Unreal_Assets\", "*.*", SearchOption.AllDirectories).Where(name => !name.EndsWith(".unity") && !name.EndsWith(".meta")).ToList();

        foreach (string dir in dirs)
        {
            var asset = Path.GetFileName(dir);

            string assetPath = "Assets" + dir.Replace(@"D:\Development\_Assets\Mir2Conquera\Mir2\Assets", "").Replace('\\', '/');

            try
            {
                foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out file))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);

                        stringBuilder.AppendLine("Asset: " + obj.name +
                            ", Type: " + obj.GetType().ToString() +
                            ", Instance ID: " + obj.GetInstanceID() +
                            ", GUID: " + guid +
                            ", File ID: " + file +
                            ", Path: " + path);
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log("Exception LoadAllAssetsAtPath: " + assetPath);
            }
        }

        File.WriteAllText(@"C:\Users\Stephen\Desktop\Mir2_Conquera\Database.txt", stringBuilder.ToString());
        Debug.Log(stringBuilder.ToString());
    }
}