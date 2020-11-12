using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepHole : RandomPlacable
{

    public override void Start()
    {
        base.Start();
    }

    public override void HandleOnUpdateCurrentChunk()
    {
        base.HandleOnUpdateCurrentChunk();

        Invoke("HandleOnEnterNewChunkDelay", 0.1f);
    }

    void HandleOnEnterNewChunkDelay()
    {
        Hide();

        if (CanSpawn())
        {
            _transform.localScale = Vector3.one;

            Show();
        }
    }

    public override void Trigger()
    {
        base.Trigger();

        int storyID = StoryLoader.Instance.FindIndexByName("Peche", StoryType.Normal);

        StoryManager newStoryManager = new StoryManager();

        newStoryManager.InitHandler(StoryType.Normal, storyID);
        StoryLauncher.Instance.PlayStory(newStoryManager, StoryLauncher.StorySource.other);

        Disappear();
    }
}
