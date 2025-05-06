using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Add at top if not already


public class ObjectDropper : MonoBehaviour
{
    [Header("Prefabs and Sprites")]
    public GameObject objectPrefab;
    public List<Sprite> possibleSprites;
    public List<Sprite> pointSprites;

    [Header("Pooling Settings")]
    public int poolSize = 50;

    [Header("Random Burst Settings")]
    public Vector2Int burstCountRange = new Vector2Int(3, 7);
    public Vector2 burstIntervalRange = new Vector2(0.1f, 0.3f);
    public Vector2 burstPauseRange = new Vector2(1.0f, 3.0f);

    [Header("Endgame Intensity Scaling")]
    [Range(0f, 1f)]
    public float endGameIntensity = 0f; // 0 = normal, 1 = max intensity

    [Header("Drop Area")]
    public Vector2 xDropRange = new Vector2(-5f, 5f);
    public float yDropHeight = 6f;

    private Queue<GameObject> objectPool = new Queue<GameObject>();
    [SerializeField] private PointTrackerUI pointTracker;
    private bool isSpawning = false;
    [Header("Drop Rate Control")]
    [Range(0f, 1f)]
    [SerializeField] private float pointDropChance = 0.3f;  // 30% chance to allow a point object

    void Start()
    {
        List<(Sprite sprite, string name, bool isPoint)> spriteAssignments = new List<(Sprite, string, bool)>();

        int totalSpriteCount = possibleSprites.Count + pointSprites.Count;

        for (int i = 0; i < poolSize; i++)
        {
            if (totalSpriteCount == 0) break;

            bool usePoint = pointSprites.Count > 0 && Random.value < 0.5f;

            if (usePoint && pointSprites.Count > 0)
            {
                int index = Random.Range(0, pointSprites.Count);
                spriteAssignments.Add((pointSprites[index], $"p_{index:00}", true));
            }
            else if (possibleSprites.Count > 0)
            {
                int index = Random.Range(0, possibleSprites.Count);
                spriteAssignments.Add((possibleSprites[index], $"{index:00}", false));
            }
        }

        foreach (var assignment in spriteAssignments)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = assignment.sprite;
            }

            obj.name = assignment.name;

            if (assignment.isPoint)
            {
                obj.tag = "points"; // Ensure this tag exists in Unity
            }

            objectPool.Enqueue(obj);
        }

        StartCoroutine(DropBurst());
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(DropBurst());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
    IEnumerator DropBurst()
    {
        while (isSpawning)
        {
            int minCount = burstCountRange.x;
            int maxCount = burstCountRange.y;
            int scaledMaxCount = Mathf.RoundToInt(Mathf.Lerp(minCount, maxCount, endGameIntensity));
            int currentBurstCount = Random.Range(minCount, scaledMaxCount + 1);

            float minPause = burstPauseRange.x;
            float maxPause = burstPauseRange.y;
            float scaledPause = Mathf.Lerp(maxPause, minPause, endGameIntensity);
            float currentBurstPause = Random.Range(scaledPause * 0.8f, scaledPause * 1.2f);

            float currentBurstInterval = Random.Range(burstIntervalRange.x, burstIntervalRange.y);

            for (int i = 0; i < currentBurstCount; i++)
            {
                GameObject obj = null;
                int attempts = 0;
                int maxTries = objectPool.Count;

                while (attempts < maxTries && objectPool.Count > 0)
                {
                    obj = objectPool.Dequeue();

                    if (obj.CompareTag("points"))
                    {
                        // Drop chance check
                        if (Random.value > pointDropChance)
                        {
                            Debug.Log($"❌ Skipped point due to rarity: {obj.name}");
                            obj.SetActive(false);
                            objectPool.Enqueue(obj);
                            obj = null;
                            attempts++;
                            continue;
                        }

                        // Skip if already collected
                        if (pointTracker != null)
                        {
                            string name = obj.name.Replace("p_", "");
                            if (int.TryParse(name, out int index) && pointTracker.CollectedIndexes.Contains(index))
                            {
                                Debug.Log($"❌ Skipping already-collected point: {obj.name}");
                                obj.SetActive(false);
                                objectPool.Enqueue(obj);
                                obj = null;
                                attempts++;
                                continue;
                            }
                        }
                    }

                    break;
                }

                if (obj != null)
                {
                    float randomX = Random.Range(xDropRange.x, xDropRange.y);
                    obj.transform.position = new Vector3(randomX, yDropHeight, 0f);
                    obj.SetActive(true);

                    Debug.Log($"Dropped object: {obj.name}");
                    StartCoroutine(DeactivateAndRecycle(obj, 10f));
                }

                yield return new WaitForSeconds(currentBurstInterval);
            }

            yield return new WaitForSeconds(currentBurstPause);
        }
    }



    IEnumerator DeactivateAndRecycle(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }
  
}
