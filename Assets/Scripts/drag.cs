using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag : MonoBehaviour
{
    // private Vector3 ResetCamera;
    public GameObject Grow;
    Vector3 Origin;
    Vector3 Diference;
    bool Drag = false;
    // void Start () {
    //     ResetCamera = Camera.main.transform.position;
    // }
    void LateUpdate () {

        if (Input.GetKey(KeyCode.Mouse0) && !Grow.GetComponent<grow>().growingStarted)
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag=true;
                Origin=Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        } 
        else
        {
            Drag = false;
        }

        if (Drag == true)
        {
            Camera.main.transform.position = Origin-Diference;
        }
        //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        // if (Input.GetMouseButton (1)) {
        //     Camera.main.transform.position = ResetCamera;
        // }
    }
 
}
