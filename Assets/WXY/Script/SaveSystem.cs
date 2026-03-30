using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UIElements;

public static class SaveSystem
{
    //存档路径
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "SaveData"); 

    //存档
    public static void SaveByJson(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        if (File.Exists(SavePath))
        {
            //这里再调用UI部分的代码里的一个方法，询问是否覆盖；选覆盖则在这行return;目前先放着不管；
            File.WriteAllText(SavePath, json);
        }
        else { File.WriteAllText(SavePath, json); }
    }
    //判断存档是否存在——>给UI显示用
    public static bool SaveFileExist()
    {
        bool exist = File.Exists(SavePath);
        return exist;
    }
    //加载存档
    public static SaveData LoadByJson()
    {
        string json;
        if (File.Exists(SavePath)) { json = File.ReadAllText(SavePath); }
        else { return null; }//后续在修改一下，要return一个默认的值===不对，只在有这个存档的时候才调用就行了
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        return saveData;
    }
    //删除存档
    public static void DeleteSaveFile()
    {
        if (File.Exists(SavePath)) { File.Delete(SavePath); }
    }
}

[Serializable]
public class SaveData
{

    public List<int> _BookBeRead;
    public IntBoolDictionary _MeshBePlaced;

    public SaveData()
    {
        _BookBeRead = new List<int>();
        _MeshBePlaced = new IntBoolDictionary();
    }
    public SaveData( List<int> bookBeRead , IntBoolDictionary meshBePlaced )
    {
        _BookBeRead = bookBeRead;
        _MeshBePlaced = meshBePlaced;
    }

}

