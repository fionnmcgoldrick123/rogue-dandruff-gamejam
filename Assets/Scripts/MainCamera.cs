using UnityEngine;

public class MainCamera : MonoBehaviour
{

    [SerializeField] private Transform playerTransform;  
    
    void Start()
    {
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    }

    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    }
}
