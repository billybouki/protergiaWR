using UnityEngine;
using UnityEngine.Events;

public class HandHoverButton : MonoBehaviour
{
    [SerializeField] private float hoverDuration = 1.0f;
    [SerializeField] private UnityEvent onHoverComplete;

    private float hoverTime = 0f;
    private bool isHovering = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hand")) // Make sure the hand object is tagged
        {
            isHovering = true;
            hoverTime = 0f;
            Debug.Log("Handndnd");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hand"))
        {
            isHovering = false;
            hoverTime = 0f;
        }
    }

    void Update()
    {
        if (isHovering)
        {
            hoverTime += Time.deltaTime;
            if (hoverTime >= hoverDuration)
            {
                isHovering = false;
                onHoverComplete.Invoke();
            }
        }
    }
}
