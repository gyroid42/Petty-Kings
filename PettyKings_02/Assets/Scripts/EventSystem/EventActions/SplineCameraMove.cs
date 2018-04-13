﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spline Camera Move", menuName = "EventActions/SplineCamMove")]
public class SplineCameraMove : BaseAction {

    // Properties

    // Spline root the camera will move through
    public GameObject splineRoot_;
    private GameObject instantiatedRoot_;

    // reference to spline controller
    private SplineController splineController;


    // Begin called at start of action
    public override void Begin(NarrativeEvent newEvent)
    {
        base.Begin(newEvent);

        // Setup reference to spline controller
        splineController = Camera.main.GetComponent<SplineController>();

        //instantiatedRoot_ = Instantiate(splineRoot_);

        // set spline root and start movement
        splineController.FollowSpline(splineRoot_);
    }


    // End called at end of action
    public override void End()
    {

        //Destroy(instantiatedRoot_);

        Debug.Log("spline move ended");
    }


    // Update called every frame action is active
    public override bool Update()
    {

        // If camera isn't moving then end the action
        if (!splineController.isMoving())
        {
            actionRunning_ = false;
        }

        return actionRunning_;
    }
}
