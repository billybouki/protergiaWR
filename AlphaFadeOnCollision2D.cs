using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AlphaFadeOnCollision2D : MonoBehaviour
{
    public float targetAlpha = 0.2f;
    public float fadeSpeed = 10f; // How fast to fade (higher = faster)

    private SpriteRenderer spriteRenderer;
    private float currentAlpha = 0f;
    private float desiredAlpha = 0f;
    [SerializeField] private PointTrackerUI pointTracker;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start fully transparent
        SetAlpha(0f);
    }

    void Update()
    {
        // Smoothly interpolate alpha
        currentAlpha = Mathf.Lerp(currentAlpha, desiredAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(currentAlpha);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hoverable hoverable = other.GetComponent<Hoverable>();
        if (hoverable != null)
        {
            if (other.name.StartsWith("p_"))
            {
                hoverable.PlayFeedbackPositive();

                if (pointTracker != null)
                {
                    string indexStr = other.name.Substring(2);
                    if (int.TryParse(indexStr, out int pointIndex))
                    {
                        pointTracker.MarkCollected(pointIndex);
                    }
                }
            }
            else
            {
                hoverable.PlayFeedbackNegative();
            }

            StartCoroutine(DelayedDeactivate(other.gameObject, 1f)); // ⏱ allow time for feedback
        }

        desiredAlpha = targetAlpha;
    }

    private IEnumerator DelayedDeactivate(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        desiredAlpha = 0f;
    }

    void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
    }
}
