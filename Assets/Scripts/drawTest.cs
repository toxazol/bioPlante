using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawTest : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;

    LineRenderer currentLineRenderer;

    List<Vector2> currentLine = new List<Vector2>();
    Vector2 lastPos;

    bool started = false;

    private void Start() {
        // var collider = brush.AddComponent<EdgeCollider2D>();
        // var lineRenderer = brush.GetComponent<LineRenderer>();
        // var positions = new Vector3[lineRenderer.positionCount];
        // lineRenderer.GetPositions(positions);
        // var points = new List<Vector2>();
        // foreach (Vector3 vector3 in positions)
        // {
        //     Vector2 vector2 = new Vector2(vector3.x, vector3.y);
        //     points.Add(vector2);
        // }
        // collider.SetPoints(points);
        // collider.edgeRadius = lineRenderer.endWidth;
    }
    
    private void Update()
    {
        Drawing();
    }

    void Drawing() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && CheckCollision())
        {
            CreateBrush();
            started = true;
        }
        else if (Input.GetKey(KeyCode.Mouse0) && started)
        {
            PointToMousePos();
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0) && currentLineRenderer != null && currentLine != null)
        {

            StartCoroutine(GrowCoroutine());

        }
    }

    IEnumerator GrowCoroutine()
    {
        foreach(Vector2 point in currentLine)
        {
            currentLineRenderer.positionCount++;
            int positionIndex = currentLineRenderer.positionCount - 1;
            currentLineRenderer.SetPosition(positionIndex, point);
            yield return new WaitForSeconds(0.01F);
        }

        EdgeCollider2D collider = currentLineRenderer.gameObject.AddComponent<EdgeCollider2D>();
        collider.SetPoints(currentLine);
        collider.edgeRadius = currentLineRenderer.endWidth;
        currentLine = new List<Vector2>();
        currentLineRenderer = null;
        started = false;
    }

    void CreateBrush() 
    {
        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);

        currentLineRenderer.SetPosition(0, mousePos);
        currentLineRenderer.SetPosition(1, mousePos);
    }

    void AddAPoint(Vector2 pointPos) 
    {
        currentLine.Add(pointPos);
        // currentLineRenderer.positionCount++;
        // int positionIndex = currentLineRenderer.positionCount - 1;
        // currentLineRenderer.SetPosition(positionIndex, pointPos);
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
            return true;
        }
        return false;
        // LineRenderer[] lineRenderers = FindObjectsOfType<LineRenderer>();
        // foreach (LineRenderer lineRenderer in lineRenderers) {
        //     var positions = new Vector3[lineRenderer.positionCount];
        //     lineRenderer.GetPositions(positions);

        // }
        // // Get all the line renderer components in the scene
        // LineRenderer[] lineRenderers = FindObjectsOfType<LineRenderer>();
        
        // // Loop through all the line renderer components
        // foreach (LineRenderer lineRenderer in lineRenderers)
        // {
        //     // Check if the brush instance collides with the line renderer
        //     if (currentLineRenderer != lineRenderer && lineRenderer.gameObject.CompareTag("Player"))
        //     {
        //         BoxCollider2D brushCollider = currentLineRenderer.gameObject.GetComponent<BoxCollider2D>();
        //         BoxCollider2D lineRendererCollider = lineRenderer.gameObject.GetComponent<BoxCollider2D>();
        //         if (brushCollider != null && lineRendererCollider != null && brushCollider.bounds.Intersects(lineRendererCollider.bounds))
        //         {
        //             Debug.Log("Collision detected with " + lineRenderer.gameObject.name);
        //         }
        //     }
        // }
    }

}