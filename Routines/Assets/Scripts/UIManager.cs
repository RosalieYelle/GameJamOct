using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public Image portraitImage;
    
    [Header("Day Panel UI")]
    public GameObject dayPanel;
    public TMP_Text dayText;
    public CanvasGroup dayCanvasGroup;

    public float fadeDuration = 0.5f;
    public float displayDuration = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ShowNewDay(int dayNum)
    {

        dayText.text = "Day " + (dayNum + 1);
        dayPanel.SetActive(true);
        StartCoroutine(ShowDayPanelRoutine());
    }

    IEnumerator ShowDayPanelRoutine()
    {
        // Fade In
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
    }

    public void UpdatePortrait(Sprite sprite)
    {
        portraitImage.sprite = sprite;
    }
}
