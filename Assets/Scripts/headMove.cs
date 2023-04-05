using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class headMove : MonoBehaviour
{
    public float moveForceFactor = 20;
    public grow Grow;
    public states States;
    GameObject movingHead;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && !Grow.isDrawing)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!CheckCollision(mousePos, "Head"))
            {
                return;
            }
            States.isHeadMoving = true;
        }
        else if(Input.GetKey(KeyCode.Mouse0) && States.isHeadMoving && !Grow.isDrawing)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var diffVector = mousePos - movingHead.transform.position;
            movingHead.GetComponent<Rigidbody2D>()
                    .AddForce(diffVector * moveForceFactor * Time.deltaTime, ForceMode2D.Force);
        } 
        else if(Input.GetKeyUp(KeyCode.Mouse0) && States.isHeadMoving)
        {
            States.isHeadMoving = false;
        }
    }

    bool CheckCollision(Vector3 position, string layerName)
    {
        int collideLayer = LayerMask.GetMask(layerName);
        var colliders = Physics2D.OverlapPointAll(position, collideLayer);

        if (colliders.Length > 0)
        {
            movingHead = colliders[0].gameObject;
            return true;
        }
        return false;
    }
}
