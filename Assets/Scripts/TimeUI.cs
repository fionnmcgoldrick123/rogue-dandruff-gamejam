using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeDisplay;

    private void Update()
    {
        if (TimeManager.Instance == null || timeDisplay == null)
            return;

        float remainingTime = TimeManager.Instance.GetRemainingTime();
        
        // Convert to minutes and seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        
        // Display in MM:SS format
        timeDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
