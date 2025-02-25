using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSwapper : MonoBehaviour
{

    public GameObject replacementPrefab;
    // Start is called before the first frame update
    public void swapObject()
    {
        print("Swapping Object!");
        Instantiate(replacementPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
