using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionTrigger : MonoBehaviour
{
    private Dictionary<GameObject, float> plantTimers = new Dictionary<GameObject, float>();
    public float requiredTime = 5.0f; // Time required for plant to grow

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            if (!plantTimers.ContainsKey(other.gameObject))
            {
                plantTimers[other.gameObject] = 0f; // Start timer if entering
            }
        }

        // Stop any ongoing coroutine for this plant
        StopCoroutine(ReduceTimerGradually(other.gameObject));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            // Increase watering time while inside
            plantTimers[other.gameObject] += Time.deltaTime;

            // Check if plant has been watered long enough
            if (plantTimers[other.gameObject] >= requiredTime)
            {

                ObjectSwapper swapper = other.GetComponent<ObjectSwapper>();
                if (swapper != null)
                {
                    swapper.swapObject();
                }
                plantTimers.Remove(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            StartCoroutine(ReduceTimerGradually(other.gameObject));
        }
    }

    private IEnumerator ReduceTimerGradually(GameObject plant)
    {
        while (plantTimers.ContainsKey(plant) && plantTimers[plant] > 0)
        {
            plantTimers[plant] -= Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Remove from tracking once time reaches zero
        if (plantTimers.ContainsKey(plant) && plantTimers[plant] <= 0)
        {
            plantTimers.Remove(plant);
        }
    }
}

               