using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class MouseClickHandler : MonoBehaviour
{

    public static MouseClickHandler instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public Vector3 GetPosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(mouseCameraRay, out float distance))
        {
            Vector3 point = mouseCameraRay.GetPoint(distance);
            // Truncate x and y to the whole number part
            Debug.Log(point);
            return new Vector3(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);

        }
        else
        {
            return Vector3.zero;
        }
    }

}

