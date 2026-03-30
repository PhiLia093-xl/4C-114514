using UnityEngine;

public class FixedHeightCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float height = 10f;
    public float mouseSensitivity = 200f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        Look();
        LockHeight();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y + mouseX, 0f);
    }

    void LockHeight()
    {
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;
    }
}