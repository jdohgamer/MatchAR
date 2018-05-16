using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SnakeController : MonoBehaviour {

    private TrackedPlane trackedPlane;

    public GameObject snakeHeadPrefab;
    private GameObject snakeInstance;

    public GameObject pointer;
    public Camera firstPersonCamera;
    // Speed to move.
    public float speed = 20f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        //check for the snake being active. 
        //If it is not, just return; there is nothing to do.
        if (snakeInstance == null || snakeInstance.activeSelf == false)
        {
            pointer.SetActive(false);
            return;
        }
        else
        {
            pointer.SetActive(true);
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

        if (Frame.Raycast(Screen.width / 2, Screen.height / 2, raycastFilter, out hit))
        {
            Vector3 pt = hit.Pose.position;
            //Set the Y to the Y of the snakeInstance
            pt.y = snakeInstance.transform.position.y;
            // Set the y position relative to the plane and attach the pointer to the plane
            Vector3 pos = pointer.transform.position;
            pos.y = pt.y;
            pointer.transform.position = pos;

            // Now lerp to the position                                         
            pointer.transform.position = Vector3.Lerp(pointer.transform.position, pt,
              Time.smoothDeltaTime * speed);
        }

        // Move towards the pointer, slow down if very close.                                                                                     
        float dist = Vector3.Distance(pointer.transform.position,
            snakeInstance.transform.position) - 0.05f;
        if (dist < 0)
        {
            dist = 0;
        }

        Rigidbody rb = snakeInstance.GetComponent<Rigidbody>();
        rb.transform.LookAt(pointer.transform.position);
        rb.velocity = snakeInstance.transform.localScale.x *
            snakeInstance.transform.forward * dist / .01f;
		
	}


    //method to set the plane
    //When the plane is set, spawn a new snake.
    public void SetPlane(TrackedPlane plane)
    {
        trackedPlane = plane;
        // Spawn a new snake.
        SpawnSnake();
    }

    //spawn the snake Set the initial position slightly above the plane 
    //so the snake drops onto the plane.
    void SpawnSnake()
    {
        if (snakeInstance != null)
        {
            DestroyImmediate(snakeInstance);
        }

        Vector3 pos = trackedPlane.CenterPose.position;
        pos.y += 0.1f;

        // Not anchored, it is rigidbody that is influenced by the physics engine.
        snakeInstance = Instantiate(snakeHeadPrefab, pos,
                Quaternion.identity, transform);

        // Pass the head to the slithering component to make movement work.
        GetComponent<Slithering>().Head = snakeInstance.transform;
    }



}
