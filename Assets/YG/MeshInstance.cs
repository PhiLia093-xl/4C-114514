using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshInstance : MonoBehaviour
{
    Vector3 middlePos; //中心点
    Vector3 upLeft; //左上
    Vector3 upRight; //右上
    Vector3 downLeft; //左下
    Vector3 downRight; //右下

    float halfLongX = 5;
    float halfLongZ = 5;

    float RealLongX;
    float RealLongZ;

    // n X m 的网格
    [SerializeField] private int n; //X轴方向长几格
    [SerializeField] private int m; //Z轴方向长几格

    Dictionary<Vector2, BuidingAndID> Buildings = new Dictionary<Vector2, BuidingAndID>();

    [SerializeField]private List<MeshCheckData> checkDataList = new List<MeshCheckData>();
    //[SerializeField] private List<MeshCheckData> UnusefulBox = new List<MeshCheckData>;
    
    [SerializeField]private Dictionary<Vector2,MeshCheckData> MeshCheckDic = new Dictionary<Vector2,MeshCheckData>();

    Dictionary<Vector2, PassCheckData> PassCheck;

    private void Start()
    {
        middlePos = transform.position;
        RealLongX = 2*halfLongX*transform.lossyScale.x;
        RealLongZ = 2*halfLongZ*transform.lossyScale.z;

        InitCheckDictionary();

        //local
        upLeft = new Vector3( -halfLongX , 0 , halfLongZ);
        upRight = new Vector3( halfLongX, 0 , halfLongZ);
        downLeft = new Vector3(- halfLongX, 0 , - halfLongZ);
        downRight = new Vector3( halfLongX, 0 ,  - halfLongZ);
        //world
        upLeft = transform.TransformPoint(upLeft);
        upRight = transform.TransformPoint(upRight);
        downLeft = transform.TransformPoint(downLeft);
        downRight = transform.TransformPoint(downRight);

    }
    private Vector2 BelongToWhichBox(Vector3 pos) 
    {
        int x = 1; //网格横坐标
        int y = 1; //网格纵坐标
        //n
        float nx = (pos.x - downLeft.x);
        for (int i = 0; i < n ; i++) 
        {
            if(nx * n <=  RealLongX* (i + 1))
            { x = i+1; break; }
        }
        //m
        float my = (pos.z - downLeft.z);
        for (int i = 0; i < m; i++)
        {
            if (my * m <= RealLongZ* (i + 1))
            { y = i + 1; break; }
        }
        return new Vector2(x, y);
    }

    private void InitCheckDictionary() 
    {
        if(checkDataList==null || checkDataList.Count == 0) 
        {
            Debug.Log("checkDataList为空或没有数据");
            return; 
        }
        MeshCheckDic = new Dictionary<Vector2, MeshCheckData> ();
        foreach(MeshCheckData data in checkDataList) 
        {
            MeshCheckDic.Add(data.NxMy, data);
        }
    }

    public void BrforePlace(Vector3 pos , int Id , GameObject Building) 
    {
        if (PassCheck == null)
        { PassCheck = new Dictionary<Vector2, PassCheckData>(); }
        MeshCheckData data;
        Vector2 Box = BelongToWhichBox(pos); //在网格的（n，m）

        if (!Buildings.ContainsKey(Box)) { Buildings.Add(Box, new BuidingAndID(Id, Building)); }

        //SaveManager.instance.SaveOnBuidingBePlaced(Id);

        if (MeshCheckDic.TryGetValue(Box, out data))
        {
            if (PassCheck.ContainsKey(Box))
            {
                if (Id != data.id)
                {
                    PassCheck[Box]._isRight = false;
                    PassCheck[Box]._beUseing = true;
                }
                else
                {
                    PassCheck[Box]._isRight = true;
                    PassCheck[Box]._beUseing = true;
                }
            }
            else 
            {

                if (Id != data.id)
                {
                    PassCheck.Add(Box, new PassCheckData(false, true));
                }
                else
                {
                    PassCheck.Add(Box, new PassCheckData(true, true));
                }
            }
            return ;
        }
        else
        {
            PassCheck.Add(Box, new PassCheckData(false, true));
            return ;
        }
    }

    public void BrforePlace(Vector2 Box , int Id , GameObject Building)
    {
        if (PassCheck == null)
        { PassCheck = new Dictionary<Vector2, PassCheckData>(); }
        MeshCheckData data;

        if (!Buildings.ContainsKey(Box))
        {
            Buildings.Add(Box, new BuidingAndID(Id , Building));
            Debug.Log($"添加==位置{Box}物体{Building}");
        }
        if (MeshCheckDic.TryGetValue(Box, out data))
        {
            if (PassCheck.ContainsKey(Box))
            {
                if (Id != data.id)
                {
                    PassCheck[Box]._isRight = false;
                    PassCheck[Box]._beUseing = true;
                }
                else
                {
                    PassCheck[Box]._isRight = true;
                    PassCheck[Box]._beUseing = true;
                }
            }
            else
            {
                if (Id != data.id)
                {
                    PassCheck.Add(Box, new PassCheckData(false, true));
                }
                else
                {
                    PassCheck.Add(Box, new PassCheckData(true, true));
                }
            }
            return;
        }
        else
        {
            PassCheck.Add(Box, new PassCheckData(false, true));
            return;
        }
    }

    public (Vector3 FianlPosition , Vector2 Box) GetPos(Vector3 p)
    {
        MeshCheckData data;
        Vector3 finalPos = new Vector3();
        Vector2 Box = BelongToWhichBox(p); //在网格的（n，m）
        if (MeshCheckDic.TryGetValue(Box, out data))
        {
            finalPos = data.pos;
            return (finalPos, Box);
        }
        else
        {
            finalPos.y = middlePos.y;
            finalPos.x = downLeft.x + (2 * Box.x - 1) * RealLongX / (2 * n);
            finalPos.z = downLeft.z + (2 * Box.y - 1) * RealLongZ / (2 * m);
            return (finalPos, Box);
        }
    }

    public bool BoxIsUsedOrNot(Vector2 Box) 
    {
        if(PassCheck == null || PassCheck.Count ==0) 
        {
            Debug.Log("还没有被记录的操作");
            return false; }
        if (PassCheck.ContainsKey(Box)) 
        {
            Debug.Log($"{Box}被占用中");
            return true;
        }
        return false;
    }
    public bool BoxIsUsedOrNot(Vector3 hitPos) 
    {
        return BoxIsUsedOrNot(BelongToWhichBox(hitPos)); 
    }

    public int OnBuildingDelet(Vector3 p) 
    {
        Vector2 Box = BelongToWhichBox(p);
        if(PassCheck==null || !PassCheck.ContainsKey(Box)) { return -1; }
        int temp = Buildings[Box].ID;
        Destroy(Buildings[Box].Buiding);
        Buildings.Remove(Box);
        PassCheck.Remove(Box);
        return temp;
    }
    public int OnBuildingDelet(Vector2 box)
    {
        if (PassCheck == null || !PassCheck.ContainsKey(box)) { return -1; }
        if(Buildings == null || !Buildings.ContainsKey(box)) { return -1; }
        Debug.Log($"移除==位置{box}物体{Buildings[box]}");
        int temp = Buildings[box].ID;

        Destroy(Buildings[box].Buiding);
        Buildings.Remove(box);

        PassCheck.Remove(box);
        return temp;
    }


    public bool CheckAll() 
    {
        if (PassCheck == null) 
        {
            Debug.Log("还没有任何操作，PassCheck为空");
            return false;
        }
        else 
        {
            foreach (KeyValuePair<Vector2,PassCheckData> IsRight in PassCheck) 
            {
                if (!IsRight.Value._isRight) 
                {
                    Debug.Log("我要验牌"+$"{IsRight.Key}");
                    return false;
                }
            }
        }
        Debug.Log("牌没有问题");
        return true;
    }
    public bool DebugCheckAll() 
    {
        bool AllRight = true;
        if (PassCheck == null)
        {
            Debug.Log("还没有任何操作，PassCheck为空");
            return false;
        }
        foreach (KeyValuePair<Vector2, PassCheckData> IsRight in PassCheck)
        {
            if (!IsRight.Value._isRight)
            {
                AllRight = false;
                Debug.Log($"{IsRight.Key}"+"上摆放着错误的建筑");
            }
        }
        if (AllRight) { Debug.Log("全对"); }
        return AllRight;
    }

}



[Serializable]
class MeshCheckData 
{
    [SerializeField]public Vector2 NxMy; //网格坐标
    [SerializeField]public int id; 
    [SerializeField]public Vector3 pos;
}

class PassCheckData 
{
    public bool _isRight = true;
    public bool _beUseing = false;
    public PassCheckData() 
    {

    }
    public PassCheckData(bool isRight , bool beUsing)
    {
        _isRight = isRight;
        _beUseing = beUsing;

    }
}

class BuidingAndID
{
    public int ID;
    public GameObject Buiding;
    public BuidingAndID(int id , GameObject buid) 
    {
        ID = id;
        Buiding = buid;
    }
}

