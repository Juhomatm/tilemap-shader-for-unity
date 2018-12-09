using UnityEngine;
using System.Collections;

public class CameraMouseController : MonoBehaviour
{
    public static bool DraggedDuringThisFrame = false;

    public Transform Target;

    private float _sign;
    private Vector3? _previousMousePosition;

    public float MinY = 1f;

    void Start()
    {
        Target = transform.GetComponent<Transform>();
        
        _sign = transform.GetComponent<Camera>() == null ? 1 : -1;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            _previousMousePosition = null;
            DraggedDuringThisFrame = false;
            return;
        }

        bool draggedDuringThisFrame = false;
        if (Input.GetMouseButton(1))
        {
            if (!_previousMousePosition.HasValue)
            {
                _previousMousePosition = Input.mousePosition;
            }
            draggedDuringThisFrame = true;
            Vector3 mouseDelta = _previousMousePosition.Value - Input.mousePosition;
            Vector3 cameraPosition = new Vector3();
            cameraPosition.x = Target.position.x + _sign * (mouseDelta.x / 3);
            cameraPosition.z = Target.position.z + _sign * (mouseDelta.y / 3);
            cameraPosition.y = Target.position.y;
            if (Target.position != cameraPosition)
            {
                draggedDuringThisFrame = true;
            }
            Target.position = cameraPosition;
            _previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (!_previousMousePosition.HasValue)
            {
                _previousMousePosition = Input.mousePosition;
            }
            Vector3 mouseDelta = _previousMousePosition.Value - Input.mousePosition;
            Vector3 cameraPosition = new Vector3();
            cameraPosition.x = Target.position.x;
            cameraPosition.z = Target.position.z;
            cameraPosition.y = Mathf.Max(MinY, Target.position.y + (mouseDelta.y / 3));
            if (Target.position != cameraPosition)
            {
                draggedDuringThisFrame = true;
            }
            Target.position = cameraPosition;
            _previousMousePosition = Input.mousePosition;
        }

        if (draggedDuringThisFrame == true)
        {
            DraggedDuringThisFrame = true;
        }
    }
}
