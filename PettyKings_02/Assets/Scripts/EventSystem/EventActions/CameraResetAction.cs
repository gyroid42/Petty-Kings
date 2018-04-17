﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ResetCamera", menuName = "EventActions/CameraReset")]
public class CameraResetAction : BaseAction
{

    // properties
    public bool clearMovement_;

    // Reference to camera controller
    private CameraController cameraController;

    // Begin method called when action starts
    public override void Begin(NarrativeEvent newEvent)
    {
        base.Begin(newEvent);

        type_ = ACTIONTYPE.CAMERARESET;

        cameraController = Camera.main.GetComponent<CameraController>();

        cameraController.Reset(clearMovement_);
    }

    // End method called when action finishes
    public override void End()
    {
        
    }


    // Called every frame the action is happening
    public override bool Update()
    {

        if (cameraController.FinishedMove())
        {
            actionRunning_ = false;
        }
        return actionRunning_;
    }
}
