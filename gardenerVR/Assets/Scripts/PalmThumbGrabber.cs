
using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class PalmThumbGrabber : MonoBehaviour
{
    [Header("Hand References")]
    public Transform palmTransform; // Reference to the palm of your hand
    public Transform thumbTransform; // Reference to the thumb (should be your currently tracked object)
    public Transform AttachTransform;
    public ArduinoController arduinoController;
    
    [Header("Grab Settings")]
    public LayerMask grabbableLayer = ~0;
    public float grabStrength = 100000f;
    public float grabDamping = 1000f;
    
    // Currently grabbed object
    private Rigidbody grabbedObject;
    private ConfigurableJoint joint;
    private bool isGrabbing = false;
    
    // Cached data
    private List<Collider> touchingPalmColliders = new List<Collider>();
    private bool thumbTouchingGrabbedObject = false;
    
    // Servo IDs
    private const int THUMB_SERVO = 0;
    private const int INDEX_SERVO = 1;
    private const int MIDDLE_SERVO = 2;
    
    // Servo positions
    private const int EXTEND_POSITION = 30;
    private const int RETRACT_POSITION = 180;
    
    void Start()
    {
        // Ensure we have references to palm and thumb
        if (palmTransform == null)
        {
            Debug.LogError("Palm transform not assigned!");
        }
        
        if (thumbTransform == null)
        {
            // Default to using this GameObject as the thumb if not specified
            thumbTransform = this.transform;
            Debug.Log("Using this GameObject as thumb transform.");
        }
        
        // Ensure the palm has a trigger collider
        Collider palmCollider = palmTransform.GetComponent<Collider>();
        if (palmCollider != null && !palmCollider.isTrigger)
        {
            Debug.LogWarning("Setting palm collider to trigger mode for detecting grabs.");
            palmCollider.isTrigger = true;
        }
        else if (palmCollider == null)
        {
            Debug.LogError("Palm has no collider! Please add a trigger collider to the palm transform.");
        }
        
        // Ensure the thumb has a collider (can be either trigger or non-trigger)
        Collider thumbCollider = thumbTransform.GetComponent<Collider>();
        if (thumbCollider == null)
        {
            Debug.LogError("Thumb has no collider! Please add a collider to the thumb transform.");
        }
    }
    
    void Update()
    {
        CheckThumbContactWithGrabbedObject();
        
        // If we're grabbing but thumb is no longer touching, release
        if (isGrabbing && !thumbTouchingGrabbedObject)
        {
            ReleaseObject();
        }
    }
    
    // Check if thumb is touching the grabbed object
    void CheckThumbContactWithGrabbedObject()
    {
        thumbTouchingGrabbedObject = false;
        
        if (isGrabbing && grabbedObject)
        {
            // Get all colliders on the grabbed object
            Collider[] objectColliders = grabbedObject.GetComponentsInChildren<Collider>();
            
            // Check if thumb is touching any of these colliders
            foreach (Collider thumbCollider in thumbTransform.GetComponents<Collider>())
            {
                foreach (Collider objCollider in objectColliders)
                {
                    if (thumbCollider.bounds.Intersects(objCollider.bounds))
                    {
                        thumbTouchingGrabbedObject = true;
                        //print("Thumb is still touching the grabbed object!");
                        return;
                    }
                }
            }
        }
    }
    
    // Called when palm collider enters another collider
    void OnTriggerEnter(Collider other)
    {
        // This should be called on the palm's collider's gameObject
        
        // Check if eligible for grabbing
        if (((1 << other.gameObject.layer) & grabbableLayer) == 0)
            return;
            
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !touchingPalmColliders.Contains(other))
        {
            touchingPalmColliders.Add(other);
            
            // Only attempt grab if we're not already grabbing something
            if (!isGrabbing)
            {
                // Check if thumb is also touching this object
                bool thumbTouching = IsThumbTouchingObject(rb);
                
                if (thumbTouching)
                {
                    GrabObject(rb);
                }
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        touchingPalmColliders.Remove(other);
    }
    
    // Check if thumb is touching the specified rigidbody
    bool IsThumbTouchingObject(Rigidbody rb)
    {
        Collider[] objectColliders = rb.GetComponentsInChildren<Collider>();
        foreach (Collider thumbCollider in thumbTransform.GetComponents<Collider>())
        {
            foreach (Collider objCollider in objectColliders)
            {
                if (thumbCollider.bounds.Intersects(objCollider.bounds))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    void GrabObject(Rigidbody rb)
    {
        if (rb == null || isGrabbing) return;

        isGrabbing = true;
        grabbedObject = rb;
        thumbTouchingGrabbedObject = true;

        Quaternion originalRotation = grabbedObject.transform.rotation;
        Vector3 initialRelativePosition = grabbedObject.transform.position - transform.position;

        // Parent the object to the hand
        grabbedObject.transform.SetParent(palmTransform);
        
        // Apply an offset (adjust as needed)
        grabbedObject.transform.localPosition = AttachTransform.localPosition; // Example offset
        grabbedObject.transform.localRotation = AttachTransform.localRotation;

        // Disable physics while held
        grabbedObject.isKinematic = true;
        
        //initiate servos
        arduinoController.MoveServo(THUMB_SERVO, EXTEND_POSITION);
        arduinoController.MoveServo(INDEX_SERVO, EXTEND_POSITION);
        arduinoController.MoveServo(MIDDLE_SERVO, EXTEND_POSITION);
        Debug.Log("Object grabbed: " + grabbedObject.name);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    void ReleaseObject()
    {
        
        if (!isGrabbing) return;

        isGrabbing = false;
        thumbTouchingGrabbedObject = false;

        // Unparent the object
        grabbedObject.transform.SetParent(null);

        // Re-enable physics (if needed)
        grabbedObject.isKinematic = false;
        
        //Retract Servos
        arduinoController.MoveServo(THUMB_SERVO, RETRACT_POSITION);
        arduinoController.MoveServo(INDEX_SERVO, RETRACT_POSITION);
        arduinoController.MoveServo(MIDDLE_SERVO, RETRACT_POSITION);
        
        Debug.Log("Released object: " + grabbedObject.name);
        grabbedObject = null;
    }
    
    // For debugging - visualize the connection
    void OnDrawGizmos()
    {
        if (isGrabbing && grabbedObject != null && palmTransform != null)
        {
            Gizmos.color = thumbTouchingGrabbedObject ? Color.green : Color.yellow;
            Gizmos.DrawLine(palmTransform.position, grabbedObject.position);
            
            if (thumbTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(thumbTransform.position, grabbedObject.position);
            }
        }
    }
    
    void FixedUpdate()
    {
        if (isGrabbing && grabbedObject != null)
        {
            // Smoothly move the bag to the hand's position and rotation
            grabbedObject.MovePosition(Vector3.Lerp(grabbedObject.position, AttachTransform.position, Time.fixedDeltaTime * 30f));
            grabbedObject.MoveRotation(Quaternion.Slerp(grabbedObject.rotation, AttachTransform.rotation, Time.fixedDeltaTime * 30f));
        }
    }
}