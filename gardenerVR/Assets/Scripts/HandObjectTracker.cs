using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class HandObjectTracker : MonoBehaviour
{
    public GameObject objectToTrack;
    public bool useRightHand = true;
    
    [Header("Positioning")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    
    private XRHandSubsystem handSubsystem;
    private XRHand targetHand;
    private XRHandJointID jointID = XRHandJointID.ThumbTip;
    
    void Start()
    {
        // Find the hand subsystem
        var xrHandSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(xrHandSubsystems);
        
        if (xrHandSubsystems.Count > 0)
        {
            handSubsystem = xrHandSubsystems[0];
        }
        else
        {
            Debug.LogError("No XR Hand Subsystem found!");
            return;
        }
        
        // Ensure we have an object to track
        if (objectToTrack == null)
        {
            objectToTrack = this.gameObject;
        }
    }
    
    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;
            
        // Get the appropriate hand
        targetHand = useRightHand ? handSubsystem.rightHand : handSubsystem.leftHand;
        
        // Check if hand is tracked
        if (targetHand.isTracked)
        {
            // Get the joint we want to track
            if (targetHand.GetJoint(jointID).TryGetPose(out var pose))
            {
                // Since the object is already a child of XR Origin, we can use the pose directly
                // Just apply position and rotation with offsets
                objectToTrack.transform.localPosition = pose.position + positionOffset;
                objectToTrack.transform.localRotation = pose.rotation * Quaternion.Euler(rotationOffset);
            }
        }
    }
}