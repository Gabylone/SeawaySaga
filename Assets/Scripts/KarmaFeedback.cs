using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KarmaFeedback : InfoFeedbacks {

    public static KarmaFeedback Instance;

    public Color bestColor;
    public Color goodColor;
    public Color neutralColor;
    public Color badColor;
    public Color worstColor;

    private void Awake()
    {
        Instance = this;
    }

    public override void Start ()
	{
		base.Start ();
	}

    public void DisplayKarma()
    {
        switch (Karma.Instance.karmaStep)
        {
            case Karma.KarmaStep.Best:
                KarmaFeedback.Instance.Print("Paragon Captain", bestColor);
                break;
            case Karma.KarmaStep.Good:
                KarmaFeedback.Instance.Print("Righteous Sailor ", goodColor);
                break;
            case Karma.KarmaStep.Neutral:
                KarmaFeedback.Instance.Print("Impartial Seaman", neutralColor );
                break;
            case Karma.KarmaStep.Bad:
                KarmaFeedback.Instance.Print("Sly Marauder (" + Karma.Instance.bounty + " gold bounty)", badColor);
                break;
            case Karma.KarmaStep.Worst:
                KarmaFeedback.Instance.Print("Wanted Pirate (" + Karma.Instance.bounty + " gold bounty)", worstColor);
                break;
        }
    }
}
