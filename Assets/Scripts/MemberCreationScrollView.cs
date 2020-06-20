using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MemberCreationScrollView : MonoBehaviour
{
    public RectTransform rectTransform;
    
    public RectTransform contentFitter;

    public GameObject memberCreationPrefab;

    public ApparenceType apparenceType;

    Vector2 initScale;

    public GridLayoutGroup gridLayout;

    public float dur = 0.2f;

    public float initCellScale = 50f;
    public float targetCellScale = 80f;

    public float speed = 1f;

    public Vector2 buttonDecal;
    public float scaleMult = 1f;

    private bool showing = false;

    public Text categoryName_Text;

    public MemberCreatorButton lastSelected;

    private void Start()
    {
        for (int i = 0; i < CrewCreator.Instance.apparenceGroups[(int)apparenceType].items.Count; ++i)
        {
            GameObject inst = Instantiate(memberCreationPrefab, contentFitter.transform) as GameObject;

            MemberCreationButton_Apparence butt = inst.GetComponent<MemberCreationButton_Apparence>();

            butt.scrollView = this;

            butt.apparenceItem.id = i;
            butt.apparenceItem.apparenceType = apparenceType;
        }

        categoryName_Text.text = "" + apparenceType.ToString();
    }

    void ShowItems()
    {
        showing = true;
    }

    void HideItems()
    {
        showing = false;
    }
}
