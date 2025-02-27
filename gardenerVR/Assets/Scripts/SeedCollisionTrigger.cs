using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCollisionTrigger : MonoBehaviour
{
    private Dictionary<GameObject, float> plantCollisions = new Dictionary<GameObject, float>();
    private int requiredCollisions = 25;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {

        print("Particle Collision Detected!");
        if (other.CompareTag("Plant"))
        {
            if (plantCollisions.ContainsKey(other.gameObject))
            {

                plantCollisions[other.gameObject] += 1;
                //print("Adiing collisions. Current number is" + plantCollisions[other.gameObject]);
            }
            else
            {
                plantCollisions.Add(other.gameObject, 1);
            }

            if (plantCollisions[other.gameObject] >= requiredCollisions)
            {
                ObjectSwapper swapper = other.GetComponent<ObjectSwapper>();
                if (swapper != null)
                {
                    swapper.swapObject();
                }
                plantCollisions.Remove(other.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
