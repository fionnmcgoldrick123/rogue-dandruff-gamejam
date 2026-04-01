using UnityEngine;

public class HairCollisionDetection : MonoBehaviour
{
    private Hair hair;

    private void Start()
    {
        hair = GetComponentInParent<Hair>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered hair trigger.");
            hair.playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited hair trigger.");
            hair.playerInside = false;
        }
    }


}
