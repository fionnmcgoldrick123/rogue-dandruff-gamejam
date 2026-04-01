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
            Cutting();
        else
            NotCutting();

    }

    private void Cutting()
    {
        Debug.Log("Cutting hair... Charge Time: " + chargeTime);
        chargeTime += Time.deltaTime;

        if (chargeTime >= cutTime)
        {
            Debug.Log("Hair Cut!");
            chargeTime = 0f;
        }
    }

    private void NotCutting()
    {
        Debug.Log("Not cutting hair... Charge Time: " + chargeTime);
        chargeTime -= Time.deltaTime;
        if (chargeTime < 0f)
            chargeTime = 0f;
    }

}
