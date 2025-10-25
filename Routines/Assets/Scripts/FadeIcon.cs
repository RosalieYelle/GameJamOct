using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIcon : MonoBehaviour
{
    public float fadeSpeed = 1f;
    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        StartCoroutine(FadeLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            // Fade out
            while (cg.alpha > 0f)
            {
                cg.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }

            // Fade in
            while (cg.alpha < 1f)
            {
                cg.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
    }
}
