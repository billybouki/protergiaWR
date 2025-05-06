using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening; // Don't forget this for DoTween

public class GeneralManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text timerText;

    [Header("Game Screens")]
    [SerializeField] private CanvasGroup introCanvas;
    [SerializeField] private CanvasGroup gameCanvas;
    [SerializeField] private CanvasGroup winCanvas;
    [SerializeField] private CanvasGroup loseCanvas;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("References")]
    [SerializeField] private ObjectDropper objectDropper;
    [SerializeField] private PointTrackerUI pointTracker;

    private bool gameStarted = false;
    private float currentTime = 0f;
    private enum GameState { Intro, Game, Win, Lose }
    private GameState currentState = GameState.Intro;
    [SerializeField] private GameObject hoverStartButtonSprite;
    [Header("Auto Screen Timing")]
    [SerializeField] private float winDuration = 3f;
    [SerializeField] private float loseDuration = 3f;
    private bool gameEnded = false;
    void Start()
    {
        SwitchToState(GameState.Intro);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchToState(GameState.Intro);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentState == GameState.Intro)
            {
                StartCoroutine(StartGameSequence());
            }
            else if (currentState == GameState.Win)
            {
                SwitchToState(GameState.Lose);
            }
        }

        if (gameStarted)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0f);
            UpdateTimerDisplay(currentTime);

            if (!gameEnded && currentTime <= 0f)
            {
                EndGame("TIME UP");
                SwitchToState(GameState.Lose);
            }

            if (!gameEnded && pointTracker.CollectedIndexes.Count >= pointTracker.TotalSlots)
            {
                EndGame("ALL COLLECTED");
                SwitchToState(GameState.Win);
            }

        }
    }

    IEnumerator StartGameSequence()
    {
        gameEnded = false;
        yield return FadeOut(introCanvas);
        SwitchToState(GameState.Game);

        timerText.text = "3";
        yield return new WaitForSeconds(1f);
        timerText.text = "2";
        yield return new WaitForSeconds(1f);
        timerText.text = "1";
        yield return new WaitForSeconds(1f);
        timerText.text = "GO!";
        yield return new WaitForSeconds(1f);

        pointTracker.ResetTracker();
        objectDropper.StartSpawning();

        gameStarted = true;
        timerText.text = $"{(int)gameDuration / 60}:{(int)gameDuration % 60:00}";
        currentTime = gameDuration;
    }

    void EndGame(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;

        gameStarted = false;
        objectDropper.StopSpawning();
        timerText.text = reason;
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes}:{seconds:00}";
    }

    void SwitchToState(GameState state)
    {
        currentState = state;

        introCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(false);
        loseCanvas.gameObject.SetActive(false);

        if (hoverStartButtonSprite != null)
            hoverStartButtonSprite.SetActive(state == GameState.Intro);

        CanvasGroup activeGroup = null;

        switch (state)
        {
            case GameState.Intro:
                activeGroup = introCanvas;
                break;
            case GameState.Game:
                activeGroup = gameCanvas;
                break;
            case GameState.Win:
                activeGroup = winCanvas;
                StartCoroutine(AutoAdvanceAfterDelay(GameState.Intro, winDuration));
                break;
            case GameState.Lose:
                activeGroup = loseCanvas;
                StartCoroutine(AutoAdvanceAfterDelay(GameState.Intro, winDuration));
                break;
        }

        if (activeGroup != null)
        {
            activeGroup.gameObject.SetActive(true);
            activeGroup.alpha = 0f;
            activeGroup.DOFade(1f, fadeDuration);
            activeGroup.interactable = true;
            activeGroup.blocksRaycasts = true;
        }
    }
    private IEnumerator AutoAdvanceAfterDelay(GameState nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchToState(nextState);
    }

    public void TriggerStartViaHover()
    {
        if (currentState == GameState.Intro)
        {
            StartCoroutine(StartGameSequence());
        }
    }
    IEnumerator FadeOut(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        yield return canvas.DOFade(0f, fadeDuration).WaitForCompletion();
        canvas.gameObject.SetActive(false);
    }

    public bool IsGameRunning() => gameStarted;

    public float GetElapsedTimeNormalized()
    {
        return Mathf.Clamp01(1f - (currentTime / gameDuration));
    }
}
