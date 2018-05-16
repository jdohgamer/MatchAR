using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScoreboardController : MonoBehaviour {

    public Camera firstPersonCamera;
    private Anchor anchor;
    private TrackedPlane trackedPlane;
    private float yOffset;
    private int score;

	// Use this for initialization
	void Start () {

        //We want the scoreboard hidden until it is anchored in position. 
        //We'll do this by disabling all the mesh renderers, then 
        //once anchored, enable them
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
        // The tracking state must be FrameTrackingState.Tracking
        // in order to access the Frame.
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        // If there is no plane, then return
        if (trackedPlane == null)
        {
            return;
        }

        // Check for the plane being subsumed.
        // If the plane has been subsumed switch attachment to the subsuming plane.
        while (trackedPlane.SubsumedBy != null)
        {
            trackedPlane = trackedPlane.SubsumedBy;
        }

        // Make the scoreboard face the viewer.
        transform.LookAt (firstPersonCamera.transform); 

        // Move the position to stay consistent with the plane.

        transform.position = new Vector3(transform.position.x,
        trackedPlane.CenterPose.position.y + yOffset, transform.position.z);
	}

    //This is called from the scene controller when the user taps a plane. 
    //When this happens, we'll create the anchor for the scoreboard
    public void SetSelectedPlane(TrackedPlane trackedPlane)
    {
        this.trackedPlane = trackedPlane;
        CreateAnchor();
    }

    void CreateAnchor()
    {
        // Create the position of the anchor by raycasting a point towards
        // the top of the screen.
        Vector2 pos = new Vector2(Screen.width * .5f, Screen.height * .90f);
        Ray ray = firstPersonCamera.ScreenPointToRay(pos);
        Vector3 anchorPosition = ray.GetPoint(5f);

        // Create the anchor at that point.
        if (anchor != null)
        {
            Destroy(anchor);
        }
        anchor = trackedPlane.CreateAnchor(
            new Pose(anchorPosition, Quaternion.identity));

        // Attach the scoreboard to the anchor.
        transform.position = anchorPosition;
        transform.SetParent(anchor.transform);

        // Record the y offset from the plane.
        yOffset = transform.position.y - trackedPlane.CenterPose.position.y;

        // Finally, enable the renderers.
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
    }
}
