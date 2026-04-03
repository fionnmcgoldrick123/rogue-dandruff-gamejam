using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {

        Cursor.visible = false;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.position = Input.mousePosition;
    }
}
