using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionTrigger : MonoBehaviour
{

    private ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (particleSystem != null)
        {
            if (other.CompareTag("Plant"))
            {
                ObjectSwapper swapper = other.GetComponent<ObjectSwapper>();
                if (swapper != null)
                {

                    print("Particle System says: Initiating swap with " + other.ToString());
                    swapper.swapObject();
                }
            }
        }
    }
}
