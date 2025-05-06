using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointTrackerUI : MonoBehaviour
{
    [SerializeField] private Image[] slots;
    [SerializeField] private Sprite[] activatedSprites;
    [SerializeField] private Sprite[] deactivatedSprites;

    public List<int> CollectedIndexes { get; private set; } = new List<int>();
    public int TotalSlots => slots.Length;

    public void ResetTracker()
    {
        CollectedIndexes.Clear();

        for (int i = 0; i < slots.Length; i++)
        {
            // Reset to corresponding deactivated sprite (fallback to null)
            if (deactivatedSprites != null && i < deactivatedSprites.Length)
                slots[i].sprite = deactivatedSprites[i];
            else
                slots[i].sprite = null;
        }
    }

    public void MarkCollected(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        if (!CollectedIndexes.Contains(index))
            CollectedIndexes.Add(index);

        if (activatedSprites != null && index < activatedSprites.Length)
            slots[index].sprite = activatedSprites[index];
    }
}
