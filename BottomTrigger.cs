using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.simulated = false; // stop falling
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
