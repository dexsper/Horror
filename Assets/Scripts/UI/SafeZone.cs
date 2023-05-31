using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void Start()
    {
        UpdateSafeArea();
    }

    private void UpdateSafeArea()
    {
        var safeArea = Screen.safeArea;
        var myRectTransform = GetComponent<RectTransform>();

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        myRectTransform.anchorMin = anchorMin;
        myRectTransform.anchorMax = anchorMax;
    }
}
