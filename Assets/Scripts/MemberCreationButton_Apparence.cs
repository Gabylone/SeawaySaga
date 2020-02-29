using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberCreationButton_Apparence : MemberCreatorButton {

    public bool raycastOnStart = false;

    public override void Start()
    {
        base.Start();

        GetComponent<Image>().raycastTarget = raycastOnStart;

    }

    public void OnPointerEnter()
    {
        Select();
    }

    public override void OnPointerUp()
    {
        base.OnPointerUp();

        if (apparenceItem.locked == false )
        {
            transform.SetAsFirstSibling();
        }
    }

    public override void Select()
    {
        base.Select();

        if (apparenceItem.locked)
            return;

        Crews.playerCrew.captain.MemberID.SetCharacterID(apparenceItem.apparenceType, apparenceItem.id);
        SoundManager.Instance.PlaySound(SoundManager.Sound.Select_Small);

        if ( apparenceItem.apparenceType == ApparenceType.genre)
        {
            if (Crews.playerCrew.captain.MemberID.GetCharacterID(ApparenceType.genre) == 0)
            {
                Crews.playerCrew.captain.MemberID.SetCharacterID(ApparenceType.hair, 3);
                Crews.playerCrew.captain.MemberID.SetCharacterID(ApparenceType.beard, 0);
            }
            else
            {
                Crews.playerCrew.captain.MemberID.SetCharacterID(ApparenceType.hair, 0);
            }

            backgroundImage.color = Color.gray;
        }

        Crews.playerCrew.captain.Icon.InitVisual(Crews.playerCrew.captain.MemberID);


    }

    public override void Deselect()
    {
        base.Deselect();

        if (apparenceItem.apparenceType == ApparenceType.genre)
        {
            backgroundImage.color = Color.white;
        }
    }


}
