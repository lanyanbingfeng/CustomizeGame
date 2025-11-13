
using UnityEngine;

public class CameraObject : MonoBehaviour
{
    public float lookPosY;
    public Transform target;
    public Vector3 startOffset;
    
    private float _cameraRotationX;
    private float _cameraRotationY;
    
    private void Start()
    {
        MonoManager.Instance.AddUpdateEvent(MyUpdate);
    }

    private void MyUpdate()
    {
        _cameraRotationX += Input.GetAxis("Mouse X");
        _cameraRotationY += Input.GetAxis("Mouse Y");
        Quaternion rotation = Quaternion.Euler(-_cameraRotationY,_cameraRotationX,0);
        Vector3 newRotation = rotation * startOffset;
        transform.position = target.position + newRotation;
        transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * lookPosY - transform.position);
    }
}
