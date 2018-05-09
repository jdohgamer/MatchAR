using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        QuitOnConnectionErrors();
	}
	
	// Update is called once per frame
	void Update () {
        if(Session.Status != SessionStatus.Tracking) 
        {
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

    void QuitOnConnectionErrors () {

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
}
