using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : MonoBehaviour
{

    private LineRenderer lineRenderer = null;
    private ParticleSystem splashParticle = null;

    private Vector3 targetPosition = Vector3.zero;

    private Coroutine pourRoutine = null;

    public float colliderRadius = 0.03f;
    private SphereCollider sphereCollider;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splashParticle = GetComponentInChildren<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public void Begin()
    {
        /*sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = colliderRadius;
        sphereCollider.isTrigger = true;*/

        StartCoroutine(UpdateParticle());
        pourRoutine = StartCoroutine(BeginPour());
    }

    private IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            targetPosition = FineEndPoint();

            MoveToPosition(0, transform.position);
            AnimateToPosition(1, targetPosition);

            yield return null;
        }
        
    }

    public void End()
    {
        StopCoroutine(pourRoutine);
        pourRoutine = StartCoroutine(EndPour());

        /*if(sphereCollider != null)
        {
            Destroy(sphereCollider.gameObject);
        }*/
    }

    private IEnumerator EndPour()
    {
        while(!HasReachedPosition(0, targetPosition))
        {
            AnimateToPosition(0, targetPosition);
            AnimateToPosition(1, targetPosition);

            yield return null;
        }

        Destroy(gameObject);
    }
    private Vector3 FineEndPoint()
    {

        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        

        Physics.Raycast(ray, out hit, 2.0f);
        Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2.0f);

        /*if (hit.collider != null)
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name);
        }*/


        return endPoint;
    }
    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(index);
        Vector3 newPosision = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * 1.75f); //Control how fast the animation moves
        lineRenderer.SetPosition(index, newPosision);
    }

    private bool HasReachedPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }

    private IEnumerator UpdateParticle()
    {
        while (gameObject.activeSelf)
        {
            splashParticle.gameObject.transform.position = targetPosition;
            bool isHitting = HasReachedPosition(1, targetPosition);
            splashParticle.gameObject.SetActive(isHitting);

            yield return null;
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            ObjectSwapper swapper = other.GetComponent<ObjectSwapper>();
            if(swapper != null)
            {

                print("Stream says: Initiating swap with " + other.ToString());
                swapper.swapObject();
            }
        }
    }*/

}
