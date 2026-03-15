using UnityEngine;
using UnityEngine.UI;

public class CloudMove : MonoBehaviour
{
    public float speedX = 0.05f;  // 水平移动速度
    public float speedY = 0.02f;  // 垂直移动速度
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
      
    }

    void Update()
    {
        if (rawImage != null)
        {
            // 移动 UV 坐标，产生云层飘动效果
            Rect rect = rawImage.uvRect;
            rect.x += speedX * Time.deltaTime;
            rect.y += speedY * Time.deltaTime;
            rawImage.uvRect = rect;
        }
    }
}