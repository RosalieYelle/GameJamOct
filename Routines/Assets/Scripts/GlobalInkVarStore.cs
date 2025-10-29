// GlobalInkVarStore.cs
using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

[DefaultExecutionOrder(-1000)] // wake up early
public class GlobalInkVarStore : MonoBehaviour
{
    public static GlobalInkVarStore Instance { get; private set; }

    // Auto-create a singleton before the first scene loads (no external bootstrap needed)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;

        // If a scene already has one placed by you, use it; otherwise create
        var existing = FindObjectOfType<GlobalInkVarStore>();
        if (existing != null)
        {
            Instance = existing;
            DontDestroyOnLoad(existing.gameObject);
        }
        else
        {
            var go = new GameObject("GlobalInkVarStore");
            Instance = go.AddComponent<GlobalInkVarStore>();
            DontDestroyOnLoad(go);
        }
    }

    // --- Hardcoded Ink variable names to sync
    private readonly string[] variableNames = new string[] {
        "break_up",
        "ending",
        "mask_bernadette",
        "mask_mateo",
        "inspired"
    };

    // Global values (Unity-side)
    private readonly Dictionary<string, object> values = new();

    // Per-story observers using Ink's delegate type (so we can remove them cleanly)
    private readonly Dictionary<Story, Dictionary<string, Story.VariableObserver>> observers = new();

    private void Awake()
    {
        // Standard singleton pattern, tolerant of pre-existing instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Public API ----------------------------------------------------------

    // Push Unity values into a freshly created Story (call right after new Story(...))
    public void PushToStory(Story story)
    {
        if (story == null) { Debug.LogError("[GlobalInkVarStore] PushToStory received null story."); return; }

        foreach (var name in variableNames)
        {
            if (values.TryGetValue(name, out var v))
            {
                SafeSet(story, name, v);
            }
        }
        StartObserving(story);
    }

    // Pull values back from a Story when you consider it "finished"
    public void PullFromStory(Story story)
    {
        if (story == null) return;

        foreach (var name in variableNames)
        {
            if (SafeGet(story, name, out var v))
            {
                values[name] = v;
            }
        }
        StopObserving(story);
    }

    // Optional helpers for other systems
    public bool TryGet(string name, out object value) => values.TryGetValue(name, out value);
    public void Set(string name, object value) => values[name] = value;

    // --- Observing -----------------------------------------------------------

    private void StartObserving(Story story)
    {
        if (!observers.ContainsKey(story))
            observers[story] = new Dictionary<string, Story.VariableObserver>();

        foreach (var name in variableNames)
        {
            // Use Ink's delegate type to match Observe/Remove signatures
            Story.VariableObserver cb = (string varName, object newVal) =>
            {
                values[varName] = NormalizeNumber(newVal);
            };

            observers[story][name] = cb;
            story.ObserveVariable(name, cb);
        }
    }

    private void StopObserving(Story story)
    {
        if (!observers.TryGetValue(story, out var map)) return;

        foreach (var kv in map)
        {
            story.RemoveVariableObserver(kv.Value, kv.Key);
        }
        observers.Remove(story);
    }

    // --- Safety / type helpers ----------------------------------------------

    private bool SafeGet(Story story, string name, out object value)
    {
        try
        {
            value = NormalizeNumber(story.variablesState[name]);
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }

    private void SafeSet(Story story, string name, object value)
    {
        try
        {
            // Probe existence so we don't throw on missing vars in some stories
            _ = story.variablesState[name];
            story.variablesState[name] = value;
        }
        catch
        {
            // Variable not present in this story — ignore
        }
    }

    private object NormalizeNumber(object v)
    {
        // Ink can box numbers as long/double depending on operations/platform
        if (v is long l) return (int)l;
        if (v is double d && Math.Abs(d % 1) < 1e-6) return (int)d;
        return v;
    }
}
