using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class grow : MonoBehaviour
{
    public Camera m_camera;
    public GameObject branch;

    private GameObject parentBranch;

    List<Vector2> currentPath = new List<Vector2>();
    Vector2 lastPos;

    bool started = false;

    public float colliderEndWidth = .16F;
    public float slowTimestep = 0.04F;
    public float dampingRatio = 1F;
    public float frequency = 10F;
    public float segmentLength = 0.5F;

    private void Start() {

    }
    
    private void Update()
    {
        Drawing();
    }

    void Drawing() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && CheckCollision())
        {
            StartPath();
            started = true;
        }
        else if (Input.GetKey(KeyCode.Mouse0) && started)
        {
            PointToMousePos();
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0) && currentPath.Count > 0)
        {
            currentPath = preparePath(currentPath);
            StartCoroutine(GrowCoroutine());

        }
    }

    List<Vector2> preparePath(List<Vector2> path)
    {
        List<Vector2> newPath = new List<Vector2>();
        newPath.Add(path[0]);
        for (int i = 1; i < path.Count; i++)
        {
            var point = path[i];
            var curVector = point - newPath.Last();
            var curVectorLen = Vector2.Distance(newPath.Last(), point);
            if(curVectorLen > segmentLength)
            {
                var newPoint = newPath.Last() + curVector * (segmentLength/curVectorLen);
                newPath.Add(newPoint);
                i--; // check next point once again because it may too far
            }
        }
        return newPath;
    }

    IEnumerator GrowCoroutine()
    {
        for (int i = 0; i + 1 < currentPath.Count; i++)
        {
            // transform
            GameObject branchSegmentInstance = Instantiate(branch);
            branchSegmentInstance.transform.position = currentPath[i];
            //do I have to set correct rotation?
            Vector2 directionVector = currentPath[i+1] - currentPath[i];

            var currentLineRenderer = branchSegmentInstance.GetComponent<LineRenderer>();
            currentLineRenderer.SetPosition(0, Vector2.zero);
            currentLineRenderer.SetPosition(1, directionVector);

            branchSegmentInstance.transform.position = currentPath[i];
            
            EdgeCollider2D collider = branchSegmentInstance.AddComponent<EdgeCollider2D>();
            collider.SetPoints(new List<Vector2> {Vector2.zero, directionVector});
            collider.edgeRadius = colliderEndWidth;
            
            Rigidbody2D rigidBody = branchSegmentInstance.AddComponent<Rigidbody2D>();
            FixedJoint2D fixedJoint = branchSegmentInstance.AddComponent<FixedJoint2D>();
            fixedJoint.dampingRatio = dampingRatio;
            fixedJoint.frequency = frequency;
            fixedJoint.connectedBody = parentBranch.gameObject.GetComponent<Rigidbody2D>();
            
            parentBranch = branchSegmentInstance;
            

            yield return new WaitForSeconds(slowTimestep);
        }

        currentPath = new List<Vector2>();
        started = false;
    }

    void StartPath() 
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        currentPath.Add(mousePos);
    }

    void AddAPoint(Vector2 pointPos) 
    {
        currentPath.Add(pointPos);
    }

    void PointToMousePos() 
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        if (lastPos != mousePos) 
        {
            AddAPoint(mousePos);
            lastPos = mousePos;
        }
    }

    bool CheckCollision()
    {
        // Create a ray from the mouse position
        Vector2 rayPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(rayPosition, Vector2.zero);

        // Check if the ray hits an object
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
        {
            parentBranch = hit.collider.gameObject;
            return true;
        }
        return false;

    }

}