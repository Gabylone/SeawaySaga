using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemberCreationButton_Apparence : MemberCreatorButton, IPointerClickHandler {

    public MemberCreationButton_Apparence memberCreationButton_Apparence;

    public override void Start()
    {
        base.Start();
    }

    public override void Select()
    {
        base.Select();

        if (apparenceItem.locked)
            return;

        SoundManager.Instance.PlaySound("click_med 04");

        Crews.playerCrew.captain.MemberID.SetCharacterID(apparenceItem.apparenceType, apparenceItem.id);
        Crews.playerCrew.captain.Icon.InitVisual(Crews.playerCrew.captain.MemberID);

        scrollView.CenterOnElement(rectTransform);

    }

    public override void Deselect()
    {
        base.Deselect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();

    }
}
