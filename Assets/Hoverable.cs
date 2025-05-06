using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

public class Hoverable : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private bool isPointElement = true;
    public int pointIndex = -1;

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player posFeedback;
    [SerializeField] private MMF_Player negFeedback;

    [Header("Events")]
    public UnityEvent<int> OnCollected;
    public UnityEvent WrongCollection;

    private bool hovered = false;

    
    public void SetIsPoint(bool value)
    {
        isPointElement = value;
    }

    public void PlayFeedbackNegative()
    {
        Debug.Log($"▶ Positive Feedback on {gameObject.name}");
        negFeedback.PlayFeedbacks();
    }
    public void PlayFeedbackPositive()
    {
        posFeedback.PlayFeedbacks();
    }



    /* void OnMouseOver()
     {
         if (hovered) return;

         hovered = true;

         if (isPointElement)
         {
             OnCollected?.Invoke(pointIndex);
         }
         else
         {
             WrongCollection?.Invoke();
         }

         outFeedback?.PlayFeedbacks();
     }*/
}
