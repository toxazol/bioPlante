using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class grow : MonoBehaviour
{
    public Camera m_camera;
    public GameObject branchSegment;
    public GameObject dottedLine;
    public growthManager growthManager;
    public GameObject head;
    public PhysicsMaterial2D catchyMaterial;

    // public float colliderEndWidth = .16F;
    public Vector2 headCenterToNeck = new Vector2(0, -0.6f);
    public float growTimestep = 0.05F;
    public float dampingRatio = 0.9F;
    public float frequency = 70;
    public float segmentLength = 0.3F;
    public float segmentMass = 0.01F;
    public float headMass = 0.05F;
    public float angularDrag = 1;
    public float linearDrag = 0;

    public bool isDrawing = false;
    bool isGrowing = false;

    GameObject parentBranchSegment;
    List<Vector2> currentPath = new List<Vector2>();
    Vector2 lastPos;
    LineRenderer dottedLineRenderer; 
    GameObject dottedLineInstance;


    private void Start() {

    }
    
    private void Update()
    {
        Drawing();
    }

    void Drawing() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) 
            && !isGrowing
            && CheckCollision(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Player"))
        {
            StartPath();
            isDrawing = true;
        }
        else if (Input.GetKey(KeyCode.Mouse0) && isDrawing)
        {
            PointToMousePos();
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0) && currentPath.Count > 0)
        {
            isDrawing = false;
            currentPath = preparePath(currentPath);
            isGrowing = true;
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
            if(growthManager.curGrowthPoints <= 0) 
            {
                StopGrowth();
                break;
            }
            growthManager.Grow(1);

            GameObject branchSegmentInstance = Instantiate(branchSegment);

            // start new segment on the end of previous one
            Vector3 directionVector = currentPath[i+1] - currentPath[i];

            if(i == 0)
            {
                branchSegmentInstance.transform.position = parentBranchSegment.transform.position 
                    + parentBranchSegment.GetComponent<LineRenderer>().GetPosition(1); 
            }
            else if(i + 2 < currentPath.Count)
            {
                Vector3 previousDirectionVector = currentPath[i] - currentPath[i-1];
                branchSegmentInstance.transform.position = parentBranchSegment.transform.position 
                    + previousDirectionVector;
            }
            else // last segment
            {
                Destroy(branchSegmentInstance);
                Vector3 previousDirectionVector = currentPath[i] - currentPath[i-1];
                SpawnHead(parentBranchSegment.transform.position + previousDirectionVector, directionVector);
                StopGrowth();
            }
           
            branchSegmentInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionVector);
            
            var currentLineRenderer = branchSegmentInstance.GetComponent<LineRenderer>();
            currentLineRenderer.SetPosition(0, Vector2.zero);
            currentLineRenderer.SetPosition(1, Vector2.up * segmentLength);
            
            
            Rigidbody2D rigidBody = branchSegmentInstance.AddComponent<Rigidbody2D>();
            rigidBody.mass = segmentMass;
            rigidBody.angularDrag = angularDrag;
            rigidBody.drag = linearDrag;
            rigidBody.sharedMaterial = catchyMaterial;
            rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            CapsuleCollider2D collider = branchSegmentInstance.AddComponent<CapsuleCollider2D>();

            // FixedJoint2D fixedJoint = branchSegmentInstance.AddComponent<FixedJoint2D>();
            // fixedJoint.dampingRatio = dampingRatio;
            // fixedJoint.frequency = frequency;
            // fixedJoint.connectedBody = parentBranchSegment.gameObject.GetComponent<Rigidbody2D>();
            // fixedJoint.autoConfigureConnectedAnchor = false;
            // fixedJoint.connectedAnchor = Vector2.up * segmentLength;

            FixedJoint2D fixedJoint1 = parentBranchSegment.AddComponent<FixedJoint2D>();
            fixedJoint1.anchor = Vector2.up * segmentLength;
            fixedJoint1.dampingRatio = dampingRatio;
            fixedJoint1.frequency = frequency;
            fixedJoint1.connectedBody = branchSegmentInstance.gameObject.GetComponent<Rigidbody2D>();
            fixedJoint1.autoConfigureConnectedAnchor = false;
            fixedJoint1.connectedAnchor = Vector2.zero;

            parentBranchSegment = branchSegmentInstance;

            yield return new WaitForSeconds(growTimestep);
        }
        StopGrowth();
    }

    void SpawnHead(Vector3 position, Vector2 direction)
    {
        var headInstance = Instantiate(head);
        
        headInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        headInstance.transform.position = position + headInstance.transform.up * headCenterToNeck.magnitude;

        Rigidbody2D rigidBody = headInstance.AddComponent<Rigidbody2D>();
        rigidBody.mass = headMass;
        rigidBody.angularDrag = angularDrag;
        rigidBody.drag = linearDrag;
        rigidBody.sharedMaterial = catchyMaterial;
        rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        CapsuleCollider2D collider = headInstance.AddComponent<CapsuleCollider2D>();
        // collider.size = new Vector2(1.43f, 1.39f); // TODO: make a circle collider?
        
        FixedJoint2D fixedJoint = parentBranchSegment.AddComponent<FixedJoint2D>();
        fixedJoint.dampingRatio = dampingRatio;
        fixedJoint.frequency = frequency;
        fixedJoint.connectedBody = rigidBody;
        fixedJoint.autoConfigureConnectedAnchor = false;
        fixedJoint.anchor = Vector2.up * segmentLength;
        fixedJoint.connectedAnchor = headCenterToNeck;
        // Time.timeScale = 0;
    }

    void StopGrowth()
    {
        Destroy(dottedLineInstance);
        currentPath = new List<Vector2>();
        isGrowing = false;
    }

    void StartPath() 
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        dottedLineInstance = Instantiate(dottedLine);
        dottedLineRenderer = dottedLineInstance.GetComponent<LineRenderer>();
        dottedLineRenderer.SetPosition(0, mousePos);
        dottedLineRenderer.SetPosition(1, mousePos);
        currentPath.Add(mousePos);
    }

    void AddAPoint(Vector2 pointPos) 
    {
        currentPath.Add(pointPos);
        dottedLineRenderer.SetPosition(dottedLineRenderer.positionCount++, pointPos);
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

    bool CheckCollision(Vector3 position, string layerName)
    {
        int collideLayer = LayerMask.GetMask(layerName);
        //TODO: optimize. All is unnecessary for grow
        var colliders = Physics2D.OverlapPointAll(position, collideLayer);

        if (colliders.Length > 0)
        {
            //TODO: parent set should be replaced by return
            //TODO: shouldnt be zero
            parentBranchSegment = colliders[0].gameObject;
            return true;
        }
        return false;

    }

}