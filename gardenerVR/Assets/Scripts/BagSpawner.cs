using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class BagSpawner : MonoBehaviour
{
    [Header("Object Spawning")]
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform spawnLocation;
    
    [Header("Input")]
    [SerializeField] private InputActionProperty spawnButton;
    
    private void OnEnable()
    {
        spawnButton.action.Enable();
        spawnButton.action.performed += SpawnObject;
    }
    
    private void OnDisable()
    {
        spawnButton.action.performed -= SpawnObject;
        spawnButton.action.Disable();
    }
    
    private void SpawnObject(InputAction.CallbackContext context)
    {
        // Only spawn if the button was pressed (not released)
        if (context.performed)
        {
            // Instantiate the object at the specified location
            GameObject newObject = Instantiate(objectToSpawn, 
                spawnLocation.position, 
                spawnLocation.rotation);
            
            Debug.Log("Object spawned at " + spawnLocation.position);
        }
    }
}