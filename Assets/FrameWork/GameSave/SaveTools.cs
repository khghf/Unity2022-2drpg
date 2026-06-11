#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
public class SaveTools
{
    [MenuItem("Tools/存档管理/打开存档文件夹")]
    public static void OpenSavePath()
    {
        EditorUtility.RevealInFinder(GameSaveSettings.GetSaveFloderPath());
    }

    [MenuItem("Tools/存档管理/清空所有本地存档 (.dat)")]
    public static void DeleteAllSaves()
    {
        string path = GameSaveSettings.GetSaveFloderPath();
        string[] files = Directory.GetFiles(path, $"*.{GameSaveSettings.SaveFileSuffix}");

        if (files.Length == 0)
        {
            Debug.Log("没有找到任何存档文件。");
            return;
        }

        foreach (var file in files)
        {
            File.Delete(file);
        }
        Debug.Log($"清理完毕！共删除了 {files.Length} 个存档。");
    }
}
#endif