using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else 
        {
            Destroy(this);
        }
    }

    private SaveData _saveData;

    private void Start()
    {
        ResetAllSaveData();//测试用
        LoadSaveFile();
    }

    public void SaveOnBookBeRead(int id) 
    {
        _saveData._BookBeRead.Add(id);
        SaveSystem.SaveByJson(_saveData);
    }

    public bool BookBeReadOrNot(int id) 
    {
        return _saveData._BookBeRead.Contains(id);
    }

    public void TestForBook() 
    {
        SaveData saveData = SaveSystem.LoadByJson();
        if (saveData == null) { Debug.Log("存档文件中SaveData为空"); }
        else 
        {
            if (saveData._BookBeRead == null) { Debug.Log("存档文件中SaveData中的book数据为空"); }
            else
            {
                if (saveData._BookBeRead.Count == 0) { Debug.Log("存档文件中显示还没有book被读过"); }
                else 
                {
                    Debug.Log("存档文件中");
                    foreach(int _id in saveData._BookBeRead) 
                    {
                        Debug.Log($"id为{_id}的书已被读完");
                    }
                }
            }
        }
        if (_saveData == null) { Debug.Log("存档管理器中SaveData为空"); }
        else
        {
            if (_saveData._BookBeRead == null) { Debug.Log("存档管理器中SaveData中的book数据为空"); }
            else
            {
                if (_saveData._BookBeRead.Count == 0) { Debug.Log("存档管理器中显示还没有book被读过"); }
                else
                {
                    Debug.Log("存档管理器中");
                    foreach (int _id in _saveData._BookBeRead)
                    {
                        Debug.Log($"id为{_id}的书已被读完");
                    }
                }
            }
        }
    }


    //==============================================
    //To Do
    //Mesh相关


    //==============================================


    

    private void LoadSaveFile() 
    {
        if (SaveSystem.SaveFileExist())
        {
            _saveData = SaveSystem.LoadByJson();
            if (_saveData._BookBeRead == null)
            { _saveData._BookBeRead = new List<int>(); }
            if (_saveData._MeshBePlaced == null)
            { _saveData._MeshBePlaced = new IntBoolDictionary(); }
        }
        else
        {
            _saveData = new SaveData();
        }
    }

    public void ResetAllSaveData()
    {
        SaveSystem.DeleteSaveFile();
        _saveData = new SaveData();
        Debug.Log("存档文件已重置");
    }

    public void ResetBookSaveData() 
    {
        _saveData._BookBeRead = new List<int>();
        SaveSystem.SaveByJson(_saveData);
    }

    public void ResetMeshSaveData()
    {
        _saveData._MeshBePlaced = new IntBoolDictionary();
        SaveSystem.SaveByJson(_saveData);
    }
}
