using UnityEngine;

public class Hair : MonoBehaviour
{
    [Header("Hair Settings")]
    [SerializeField] private float cutTime;

    private float chargeTime;
    public bool playerInside { get; set; }

    private void Start()
    {
    }

    private void Update()
    {
        if (playerInside)
            BeginCutting();
    }

    private void BeginCutting()
    {
        chargeTime += Time.deltaTime;

        if (chargeTime >= cutTime)
        {
            Debug.Log("Hair Cut!");
            chargeTime = 0f;
        }
    }

}
