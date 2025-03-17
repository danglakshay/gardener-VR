using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class PalmThumbGrabber : MonoBehaviour
{
    [Header("Hand References")]
    public Transform palmTransform; // Reference to the palm of your hand
    public Transform thumbTransform; // Reference to the thumb (should be your currently tracked object)
    public Transform AttachTransform;
    
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
                        print("Thumb is still touching the grabbed object!");
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

        // Parent the object to the hand
        grabbedObject.transform.SetParent(palmTransform);
        
        // Apply an offset (adjust as needed)
        grabbedObject.transform.localPosition = AttachTransform.localPosition; // Example offset
        grabbedObject.transform.localRotation = Quaternion.Euler(0, 30, 90); // Reset rotation

        // Disable physics while held
        grabbedObject.isKinematic = true;

        Debug.Log("Object grabbed: " + grabbedObject.name);

        // if (rb == null || isGrabbing) return;
        //
        // // Destroy any existing joint
        // if (joint != null)
        // {
        //     Destroy(joint);
        //     joint = null;
        // }
        //
        // print("ATTEMPTING GRAB");
        //
        // isGrabbing = true;
        // grabbedObject = rb;
        // thumbTouchingGrabbedObject = true; // Initially true since we just grabbed it
        //
        // // Create a joint on the palm to "weld" the object to the palm
        // joint = thumbTransform.gameObject.AddComponent<ConfigurableJoint>();
        // joint.connectedBody = grabbedObject;
        //
        // // Configure the joint to hold the object firmly
        // joint.xMotion = ConfigurableJointMotion.Locked;
        // joint.yMotion = ConfigurableJointMotion.Locked;
        // joint.zMotion = ConfigurableJointMotion.Locked;
        // joint.angularXMotion = ConfigurableJointMotion.Locked;
        // joint.angularYMotion = ConfigurableJointMotion.Locked;
        // joint.angularZMotion = ConfigurableJointMotion.Locked;
        //
        // // Set up the joint drive
        // JointDrive drive = new JointDrive
        // {
        //     positionSpring = grabStrength,
        //     positionDamper = grabDamping,
        //     maximumForce = Mathf.Infinity
        // };
        //
        // joint.xDrive = drive;
        // joint.yDrive = drive;
        // joint.zDrive = drive;
        // joint.angularXDrive = drive;
        // joint.angularYZDrive = drive;
        //
        // // Enhance physics behavior while held
        // if (grabbedObject != null)
        // {
        //     grabbedObject.interpolation = RigidbodyInterpolation.Interpolate;
        //     grabbedObject.collisionDetectionMode = CollisionDetectionMode.Continuous;
        // }
        //
        // Debug.Log("Object grabbed by palm: " + grabbedObject.name);
    }
    
    void ReleaseObject()
    {
        
        if (!isGrabbing) return;

        isGrabbing = false;
        thumbTouchingGrabbedObject = false;

        // Unparent the object
        grabbedObject.transform.SetParent(null);

        // Re-enable physics (if needed)
        grabbedObject.isKinematic = false;

        Debug.Log("Released object: " + grabbedObject.name);
        grabbedObject = null;
        // if (!isGrabbing || !joint) return;
        //
        // isGrabbing = false;
        // thumbTouchingGrabbedObject = false;
        //
        // // Remove the joint
        // Destroy(joint);
        // joint = null;
        //
        // // Reset the grabbed object
        // if (grabbedObject)
        // {
        //     Debug.Log("Released object: " + grabbedObject.name);
        //     grabbedObject = null;
        // }
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
            grabbedObject.MovePosition(Vector3.Lerp(grabbedObject.position, palmTransform.position, Time.fixedDeltaTime * 10f));
            grabbedObject.MoveRotation(Quaternion.Slerp(grabbedObject.rotation, palmTransform.rotation, Time.fixedDeltaTime * 10f));
        }
    }
}