using System;
using UnityEngine;

public class HoldDetector : MonoBehaviour
{

    public Transform attachPoint;
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    private void OnCollisionEnter(Collision other)
    {
        print("Detected collision with" + other.gameObject.name);
        if (other.gameObject.CompareTag("Thumb"))
        {
            print("Found correct tag, initiating attach");
            AttachToHand();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Thumb"))
        {
            DetachFromHand();
        }
    }

    private void AttachToHand()
    {
        if (attachPoint == null)
        {
            print("attachPoint is null");
            return;
        }
        
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void DetachFromHand()
    {
        transform.SetParent(null);
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
