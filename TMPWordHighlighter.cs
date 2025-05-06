using UnityEngine;
using TMPro;

public class TMPWordHighlighter : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField, TextArea] private string fullText = "Ευχαριστούμε πολύ για την ενέργεια σου";
    [SerializeField] private string wordToHighlight = "ενέργεια";
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float sizeMultiplier = 1.1f; // 10% bigger

    void Start()
    {
        if (targetText == null || string.IsNullOrEmpty(wordToHighlight))
            return;

        string hexColor = ColorUtility.ToHtmlStringRGB(highlightColor);
        string highlighted = $"<size={sizeMultiplier * 100}%><color=#{hexColor}>{wordToHighlight}</color></size>";
        string result = fullText.Replace(wordToHighlight, highlighted);

        targetText.text = result;
    }
}
