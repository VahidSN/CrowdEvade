using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector2 zoomRage = new Vector2(8f, 55f);

    private Vector3 lookDirection;
    private float zoomAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lookDirection = (transform.position - target.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        zoomAmount = Mathf.Clamp(zoomAmount - Input.mouseScrollDelta.y, zoomRage.x, zoomRage.y);
        transform.position = target.position + lookDirection * zoomAmount;
    }
}
