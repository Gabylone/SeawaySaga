﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepHole : RandomPlacable
{

    public override void Start()
    {
        base.Start();
    }

    public override void HandleOnEnterNewChunk()
    {
        base.HandleOnEnterNewChunk();

        transform.localScale = Vector3.one;

        CheckProximityWithPlayer();

        
    }

    public override void Trigger()
    {
        base.Trigger();

        int storyID = StoryLoader.Instance.FindIndexByName("Fishing", StoryType.Normal);

        StoryManager newStoryManager = new StoryManager();

        newStoryManager.InitHandler(StoryType.Normal, storyID);
        StoryLauncher.Instance.PlayStory(newStoryManager, StoryLauncher.StorySource.other);

        Disappear();
    }
}