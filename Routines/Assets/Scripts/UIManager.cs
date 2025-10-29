using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public Image portraitImage;
    public Image expressionImage; 
    
    [Header("Day Panel UI")]
    public GameObject dayPanel;
    public TMP_Text dayText;
    public CanvasGroup dayCanvasGroup;

    public float fadeDuration = 0.3f;
    public float displayDuration = 2f;
    public float doneAt = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        fadeDuration = 0.7f;
        dayPanel.SetActive(false);
    }

    public void ShowNewDay(int dayNum)
    {
        dayText.text = "Day " + (dayNum + 1);
        dayPanel.SetActive(true);
        StartCoroutine(ShowDayPanelRoutine());
    }

    public void ShowFirstDay()
    {
        
        dayPanel.SetActive(true);
        StartCoroutine(ShowFirstDayPanelRoutine());
    }

    public void ShowEnd()
    {
        //doneAt = Time.time + fadeDuration * 2 + displayDuration;
        dayText.text = "The End";
        dayPanel.SetActive(true);
        displayDuration = 100f;
        dayCanvasGroup.alpha = 1f;
        //StartCoroutine(ShowDayPanelRoutine());
        
    }

    IEnumerator ShowDayPanelRoutine()
    {
        // Fade In
        
        FindFirstObjectByType<PlayerController>().canMove = false;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dayCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dayCanvasGroup.alpha = 1f - (t / fadeDuration);
            yield return null;
        }

        dayPanel.SetActive(false);
        FindFirstObjectByType<PlayerController>().canMove = true;
    }

    IEnumerator ShowFirstDayPanelRoutine()
    {
               // Fade In
        dayText.text = "ROUTINES";
        FindFirstObjectByType<PlayerController>().canMove = false;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dayCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        t = 0f;
        while (t < fadeDuration / 2)
        {
            t += Time.deltaTime;
            dayText.alpha = 1f - t / fadeDuration;
            yield return null;
        }

        dayText.text = "Day 1";

        t = 0f;
        while (t < fadeDuration / 2)
        {
            t += Time.deltaTime;
            dayText.alpha = t / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dayCanvasGroup.alpha = 1f - (t / fadeDuration);
            yield return null;
        }

        dayPanel.SetActive(false);
        FindFirstObjectByType<PlayerController>().canMove = true;
    }

    public void UpdatePortrait(Sprite sprite)
    {
        portraitImage.sprite = sprite;
        expressionImage.sprite = sprite;
    }

}
