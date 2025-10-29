using System.Collections;
using UnityEngine;

public class CrossfadeAudioManager : MonoBehaviour
{
    [Tooltip("Assign one AudioSource per track (order matters).")]
    public AudioSource[] sources;

    [Tooltip("0-based index of the track that should play at Start.")]
    public int defaultTrack = 0;

    [Tooltip("Seconds to fade between tracks.")]
    public float fadeDuration = 1.0f;

    int current = -1;
    Coroutine fadeCoroutine = null;

    void Start()
    {
        if (sources == null || sources.Length == 0)
        {
            Debug.LogError("SimpleCrossfadeMusic: no AudioSources assigned.");
            return;
        }

        // Ensure loop and initial play state:
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] == null) continue;
            sources[i].loop = true;
            sources[i].volume = 0f;
            sources[i].Stop();
            // make sure component/GameObject is enabled so you can see it in Inspector
            sources[i].enabled = false;
            sources[i].gameObject.SetActive(true); // keeps source visible in hierarchy
        }

        // Start the default track immediately
        defaultTrack = Mathf.Clamp(defaultTrack, 0, sources.Length - 1);
        current = defaultTrack;
        var s = sources[current];
        s.enabled = true;
        s.volume = 1f;
        if (!s.isPlaying) s.Play();
    }

    /// <summary>
    /// Public method to request a track (0-based).
    /// </summary>
    public void PlayTrack(int index)
    {
        if (sources == null || index < 0 || index >= sources.Length)
        {
            Debug.LogWarning($"PlayTrack: invalid index {index}");
            return;
        }

        if (index == current) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(current, index, fadeDuration));
        current = index;
    }

    IEnumerator FadeRoutine(int fromIndex, int toIndex, float duration)
    {
        AudioSource from = (fromIndex >= 0 && fromIndex < sources.Length) ? sources[fromIndex] : null;
        AudioSource to = sources[toIndex];

        // Ensure 'to' is enabled and playing
        to.enabled = true;
        to.volume = 0f;
        if (!to.isPlaying) to.Play();

        float start = Time.time;
        float end = start + Mathf.Max(0.0001f, duration);

        float fromStart = (from != null) ? from.volume : 0f;
        float toStart = 0f;

        while (Time.time < end)
        {
            float t = (Time.time - start) / duration; // linear
            if (from != null) from.volume = Mathf.Lerp(fromStart, 0f, t);
            to.volume = Mathf.Lerp(toStart, 1f, t);
            yield return null;
        }

        // finalize
        if (from != null)
        {
            from.volume = 0f;
            from.Stop();              // actually stop playback
            from.enabled = false;     // disable the AudioSource component so inspector reflects change
        }

        to.volume = 1f;
        fadeCoroutine = null;
    }

    // Optional quick helpers for Inspector testing:
    [ContextMenu("Play Next")]
    void DebugNext()
    {
        PlayTrack((current + 1) % sources.Length);
    }
}