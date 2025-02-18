using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinkleDetector : MonoBehaviour
{

    public int sprinkleThreshold = 45;
    public Transform origin = null;
    public ParticleSystem seedParticle = null;

    private bool isSprinkling = false;


    // Start is called before the first frame update
    void Start()
    {
        if (seedParticle != null)
        {
            seedParticle.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool sprinkleCheck = CalculateAngle() < sprinkleThreshold;
        //print("Angle: "+  CalculateAngle());

        if (isSprinkling != sprinkleCheck)
        {
            isSprinkling = sprinkleCheck;

            if (isSprinkling)
            {
                StartSprinkle();
            }
            else
            {
                EndSprinkle();
            }
        }
    }

    private void StartSprinkle()
    {
        print("Start");
        if (seedParticle != null)
        {
            seedParticle.Play();
            
        }
    }

    private void EndSprinkle ()
    {

        print("End");
        if (seedParticle != null)
        {
            seedParticle.Stop();
            print("Stopped seeds");
        }
    }

    private float CalculateAngle()
    {
        return transform.up.y * Mathf.Rad2Deg;
    }

    
}
