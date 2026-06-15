using OdinSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameSaveSettings
{
    public static readonly string SaveFileName = "GameSave";
    public static readonly string SaveFileSuffix = "dat";
    public static readonly DataFormat dataFormat = DataFormat.JSON;
    /// <summary>
    /// 获取存档文件夹路径
    /// </summary>
    /// <returns></returns>
    public static string GetSaveFloderPath()
    {
        return Path.Combine(Application.persistentDataPath, "GameSave");
    }
}



public class GameSave<T>where T : GameSaveData,new()
{
    protected static int CurSlotIndex = -1;
    protected static T CurSaveData = null;

    protected static List<SaveSlot<T>>SaveSlots = new List<SaveSlot<T>>();

    /// <summary>
    /// 获取指定存档槽位的存档数据
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public static T GetSaveData(int slotIndex)
    {
        if (SaveSlots==null||SaveSlots.Count==0)
        {
            foreach(var data in GetAllSlotSaveData())
            {
                SaveSlots.Add(new SaveSlot<T>(data));
            }
        }
        if (!IsValidSlotIndex(slotIndex)) return null;
        return SaveSlots[slotIndex].Data;
    }
    /// <summary>
    /// 获取当前游戏存档数据
    /// </summary>
    /// <returns></returns>
    public static T GetCurSaveData()
    {
        return CurSaveData;
    }

    /// <summary>
    /// 创建一个新的存档槽位
    /// </summary>
    /// <returns></returns>
    public static T CreateNewSlot()
    {
        T saveData = new T();
        SaveSlot<T> slot = new SaveSlot<T>(null);
        slot.Data=saveData;
        SaveSlots.Add(slot);
        saveData.SlotIndex=SaveSlots.Count-1;
        return saveData;
    }

    /// <summary>
    /// 判断是否为有效的存档槽位
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public static bool IsValidSlotIndex(int slotIndex)
    {
        if(slotIndex<0)return false;
        if(slotIndex>=SaveSlots.Count)return false;
        return true;
    }

    /// <summary>
    /// 获取指定存档槽位的存档文件路径
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    private static string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(GameSaveSettings.GetSaveFloderPath(), $"{GameSaveSettings.SaveFileName}_{slotIndex}.{GameSaveSettings.SaveFileSuffix}");

    }

    /// <summary>
    /// 获取所有的存档文件的路径，并按照槽位(slotIndex)从小到大排序
    /// </summary>
    public static List<string> GetAllSavePathsSorted()
    {
        string dirPath = GameSaveSettings.GetSaveFloderPath();

        string[] files = Directory.GetFiles(dirPath, $"{GameSaveSettings.SaveFileName}_*.{GameSaveSettings.SaveFileSuffix}");

        List<string> sortedPaths = files.OrderBy(path =>
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            string numberStr = fileName.Replace($"{GameSaveSettings.SaveFileName}_", "");

            if (int.TryParse(numberStr, out int slotIndex))
            {
                return slotIndex;
            }
            return int.MaxValue;

        }).ToList();
        return sortedPaths;
    }



    /// <summary>
    /// 获取当前存档文件路径 (.dat)
    /// </summary>
    private static string GetCurSaveFilePath()
    {
        return GetSaveFilePath(CurSlotIndex);
    }

   

    /// <summary>
    /// 获取所有存档槽位的存档数据
    /// </summary>
    /// <returns></returns>
    public static List<T> GetAllSlotSaveData()
    {
        List<T> saveDatas = new List<T>();

        List<string>Paths=GetAllSavePathsSorted();
        foreach(var path in Paths)
        {
            byte[] bytes = File.ReadAllBytes(path);

            T saveData =SerializationUtility.DeserializeValue<T>(bytes, GameSaveSettings.dataFormat);
            if(saveData==null)
            {
                LogWarning($"存档加载失败[{path}]");
            }
            else
            {
                saveDatas.Add(saveData);
            }
        }
        saveDatas=saveDatas.OrderBy(data => { return data.SlotIndex; }).ToList();
        return saveDatas;
    }


    /// <summary>
    /// 执行存档：写入硬盘
    /// </summary>
    public static void SaveGameData()
    {
        try
        {
            string path = GetCurSaveFilePath();
            //确保存档目录的存在
            if(!Directory.Exists(GameSaveSettings.GetSaveFloderPath()))
            {
                Directory.CreateDirectory(GameSaveSettings.GetSaveFloderPath());
            }
            byte[] bytes = SerializationUtility.SerializeValue(GetCurSaveData(), GameSaveSettings.dataFormat);
            //确保存档文件的存在
            //if(!File.Exists(path))
            //{
            //    File.Create(path).Close();
            //}

            File.WriteAllBytes(path, bytes);
            Debug.Log($"[SaveManager] 游戏保存成功！槽位: {CurSlotIndex}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 保存失败: {e.Message}");
        }
    }

    /// <summary>
    /// 执行读档：读取硬盘
    /// </summary>
    public static T LoadGameData(int slotIndex=0)
    {
        CurSlotIndex=slotIndex;
        string path = GetSaveFilePath(CurSlotIndex);
        T saveData = GetSaveData(CurSlotIndex);
        if (saveData==null)
        {
            Debug.LogWarning($"[SaveManager] 找不到槽位 {CurSlotIndex} 的存档，将使用默认全新数据。");
            saveData=CreateNewSlot();
            CurSlotIndex =saveData.SlotIndex;
        }
        else
        {
            try
            {

                saveData=GetSaveData(CurSlotIndex);
                Debug.Log($"[SaveManager] 读档成功！槽位: {slotIndex}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 存档损坏或版本不兼容: {e.Message}。已强制重置数据。");
            }
        }
        CurSaveData = saveData;
        return CurSaveData;
    }

    /// <summary>
    /// 释放所有存档槽位的存档数据内存(除了当前正在游玩的存档)
    /// </summary>
    public static void ClearAllSlot()
    {
        SaveSlots.Clear();
        SaveSlots=null;
    }

    private static void LogInfo(string msg)
    {
        Debug.Log($"[SaveMgr] {msg}");
    }
    private static void LogWarning(string msg)
    {
         Debug.LogWarning($"[SaveMgr] {msg}");
    }
    private static void LogError(string msg)
    {
        Debug.LogError($"[SaveMgr] {msg}");
    }

}