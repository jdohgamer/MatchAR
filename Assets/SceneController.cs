using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour
{

    public GameObject trackedPlanePrefab;

    public Camera firstPersonCamera;

    public ScoreboardController scoreboard;

    public SnakeController snakeController;


    // Use this for initialization
    void Start()
    {
        QuitOnConnectionErrors();
    }

    // Update is called once per frame
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ProcessNewPlanes();

        ProcessTouches();
    }

    void QuitOnConnectionErrors()
    {

        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                "Woah bro, we need some cam permits..", 5));
        }
        else if (Session.Status.IsError())
        {
            //This covers a variety of errors. See reference for details
            // https://developers.google.com/ar/reference/unity/namespace/GoogleARCore
            StartCoroutine(CodelabUtils.ToastAndExit(
                "ARCore couldn't connect to your thingamuhjig bruh. Restart the app.", 5));
        }
    }

    //method to iterate through the newly detected planes 
    //and creates a game object to visualize the plane
    void ProcessNewPlanes()
    {

        List<TrackedPlane> planes = new List<TrackedPlane>();
        Session.GetTrackables(planes, TrackableQueryFilter.New);

        for (int i = 0; i < planes.Count; i++)
        {
            // Instantiate a plane visualization prefab and set it to track the new plane.
            // The transform is set to the origin with an identity rotation since the mesh
            // for our prefab is updated in Unity World coordinates.
            GameObject planeObject = Instantiate(trackedPlanePrefab, Vector3.zero,
                                                 Quaternion.identity, transform);
            planeObject.GetComponent<TrackedPlaneController>().SetTrackedPlane(planes[i]);

        }

    }

    //his method will perform the ray casting hit test and select the plane that is tapped.
    void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount != 1 ||
            (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter =
            TrackableHitFlags.PlaneWithinBounds |
            TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            SetSelectedPlane(hit.Trackable as TrackedPlane);

        }
    }

    //This is used to notify all the other controllers that a new plane has been selected. Right now it just logs we selected a plane.
    void SetSelectedPlane(TrackedPlane selectedPlane)
    {
        //Debug.Log("Selected plane centered at " + selectedPlane.CenterPose.position);
        scoreboard.SetSelectedPlane(selectedPlane);
        snakeController.SetPlane(selectedPlane);
    }


}