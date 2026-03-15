using UnityEngine;

public class RotateIcon : MonoBehaviour
{
    public float speed = 90f; // 旋转速度，可调

    void Update()
    {
        transform.Rotate(0, 0, -speed * Time.deltaTime);
    }
}