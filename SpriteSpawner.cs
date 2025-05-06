using System.Collections.Generic;
using UnityEngine;

public class SpriteSpawner : MonoBehaviour
{
    [Header("Prefabs & Sprites")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Sprite[] pointSprites;

    [SerializeField] private GameObject nonPointPrefab;
    [SerializeField] private Sprite[] nonPointSprites;

    [Header("Spawn Area")]
    [SerializeField] private GameObject background;
    [SerializeField] private float verticalOffset = 2f;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField, Range(0f, 1f)] private float nonPointChance = 0.3f;

    private List<GameObject> pointObjects = new List<GameObject>();
    private List<GameObject> nonPointObjects = new List<GameObject>();
    private float timer = 0f;

    private List<GameObject> dropOrder = new List<GameObject>();
    private int dropIndex = 0;
    void Start()
    {
        SpawnAllPointElements();
        SpawnAllNonPointElements();
        dropOrder.AddRange(pointObjects);
        dropOrder.AddRange(nonPointObjects);
        ShuffleDropOrder();
    }
    void ShuffleDropOrder()
    {
        for (int i = dropOrder.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (dropOrder[i], dropOrder[j]) = (dropOrder[j], dropOrder[i]);
        }

        dropIndex = 0;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            DropNextElement();
            timer = 0f;
        }
    }

    void SpawnAllPointElements()
    {
        for (int i = 0; i < pointSprites.Length; i++)
        {
            GameObject obj = Instantiate(pointPrefab);
            obj.GetComponent<SpriteRenderer>().sprite = pointSprites[i];

            Hoverable hoverable = obj.GetComponent<Hoverable>();
            hoverable.pointIndex = i;

            pointObjects.Add(obj);
        }
        
    }

    void SpawnAllNonPointElements()
    {
        for (int i = 0; i < nonPointSprites.Length; i++)
        {
            GameObject obj = Instantiate(nonPointPrefab);
            obj.GetComponent<SpriteRenderer>().sprite = nonPointSprites[i];

            Hoverable hoverable = obj.GetComponent<Hoverable>();
            hoverable.pointIndex = -1;

            nonPointObjects.Add(obj);
        }
    }

    void DropNextElement()
    {
        if (dropOrder.Count == 0) return;

        // First, check if all point objects are gone
        bool allPointsGone = true;
        foreach (var obj in pointObjects)
        {
            if (obj != null)
            {
                allPointsGone = false;
                break;
            }
        }

        if (allPointsGone)
        {
            Debug.Log("Game Over: All point elements have been used.");
            enabled = false; // stop this script from running
            return;
        }

        // Start cycling
        dropIndex++;
        dropIndex %= dropOrder.Count;

        // Try to find a valid object to drop
        int attempts = 0;
        while (attempts < dropOrder.Count)
        {
            GameObject obj = dropOrder[dropIndex];
            if (obj != null)
            {
                SetToTopPosition(obj);
                return;
            }

            // Try next
            dropIndex++;
            dropIndex %= dropOrder.Count;
            attempts++;
        }

        // No available object found (all were null), do nothing
    }

    void SetToTopPosition(GameObject obj)
    {
        SpriteRenderer bg = background.GetComponent<SpriteRenderer>();
        float minX = bg.bounds.min.x;
        float maxX = bg.bounds.max.x;

        float x = Random.Range(minX, maxX);
        float y = bg.bounds.max.y + verticalOffset;

        obj.transform.position = new Vector2(x, y);

        // Reset velocity
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }


}
