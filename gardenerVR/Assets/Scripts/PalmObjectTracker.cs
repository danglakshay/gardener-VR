using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class PalmObjectTracker : MonoBehaviour
{
    [Header("Hand Settings")]
    public bool useRightHand = true;
    
    [Header("Positioning")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    
    private XRHandSubsystem handSubsystem;
    private XRHand targetHand;
    private XRHandJointID jointID = XRHandJointID.Palm; // Track the palm joint
    
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
        
        // Make sure we have a collider for interaction
        if (GetComponent<Collider>() == null)
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.05f;
            collider.isTrigger = true;
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
            // Get the joint we want to track (Palm)
            if (targetHand.GetJoint(jointID).TryGetPose(out var pose))
            {
                // Since the object is a child of XR Origin, we use local space
                transform.localPosition = pose.position + positionOffset;
                transform.localRotation = pose.rotation * Quaternion.Euler(rotationOffset);
            }
        }
    }
    
    // For debugging
    void OnDrawGizmos()
    {
        // Draw a sphere to represent the palm position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.02f);
    }
}