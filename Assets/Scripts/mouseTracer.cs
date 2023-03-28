using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseTracer : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Update()
    {
     Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
     mouseWorldPosition.z = 0f;
     transform.position = mouseWorldPosition;   
    }
}
