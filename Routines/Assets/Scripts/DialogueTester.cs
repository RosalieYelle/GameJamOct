using UnityEngine;
using Ink.Runtime;

public class DialogueTester : MonoBehaviour
{
    public TextAsset inkJSON; // Drag your .ink file here
    private Story story;

    void Start()
    {
        story = new Story(inkJSON.text);
        ContinueStory();
    }

    void ContinueStory()
    {
        while(story.canContinue)
        {
            Debug.Log(story.Continue());
        }
    }
}
