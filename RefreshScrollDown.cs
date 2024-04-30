using UnityEngine;
using UnityEngine.UI;

public class RefreshScrollDown : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float debugThreshold; // Minimum distance from the top to trigger debug log

    void Start()
    {
        // Subscribe to the onValueChanged event of ScrollRect
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    void OnScrollValueChanged(Vector2 normalizedValue)
    {
        // Check if scrolled up from the top
        if (normalizedValue.y > 0 && scrollRect.normalizedPosition.y > 0)
        {
            // Calculate the content position delta based on normalized value
            float contentPositionDelta = scrollRect.content.anchoredPosition.y - (scrollRect.content.rect.height - scrollRect.viewport.rect.height);

            // Check if delta is above threshold
            if (contentPositionDelta >= debugThreshold)
            {
                Debug.Log(contentPositionDelta);
            }
        }
    }
}
