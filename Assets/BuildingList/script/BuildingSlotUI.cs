
/*
 * 该类用于描述建筑格子
 * 实现格子的基本功能
*/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BuildingSlotUI : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{

    [Header("格子内容属性")]
    public Image iconImage;
    private BuildingData data;
    private GameObject ghostInstance;

    public BuildingEvent buildingEvent;
    public void Init(BuildingData buildingData)
    {
        data = buildingData;
        iconImage.sprite = data.icon;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("建筑被选择");
        if (data.prefab == null) return;//如果没有预制体，就跳过

        ghostInstance = Instantiate(data.prefab);//有prefab属性，初始化
    }
    public void OnDrag(PointerEventData eventData)
    {

        Debug.Log("建筑被拖动");
        if (ghostInstance == null) return; //没有预制体，跳过

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//射线检测
        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            //网格对齐逻辑    
            Vector3 pos = new Vector3(Mathf.Round(hit.point.x), 0, Mathf.Round(hit.point.z));
            ghostInstance.transform.position = pos;

        }

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("建筑被放置");
        //测试：松开销毁或者留在原地
        ghostInstance = null;
        if(buildingEvent!=null) buildingEvent.Raise(data);//触发放置事件
    }

}
