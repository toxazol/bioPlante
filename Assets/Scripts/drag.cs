using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag : MonoBehaviour
{
    public grow Grow;
    public states States;
    Vector3 Origin;
    Vector3 Diference;
    bool Drag = false;


    void LateUpdate () {

        if (Input.GetKey(KeyCode.Mouse0) 
            && !Grow.isDrawing
            && !States.isHeadMoving)
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

    }
 
}
