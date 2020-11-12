using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MemberCreationScrollView : MonoBehaviour
{
    public RectTransform rectTransform;

    public RectTransform scrollView_RectTransform;
    public RectTransform content_RectTransform;

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
            GameObject inst = Instantiate(memberCreationPrefab, content_RectTransform.transform) as GameObject;

            MemberCreationButton_Apparence butt = inst.GetComponent<MemberCreationButton_Apparence>();

            butt.scrollView = this;

            butt.apparenceItem.id = i;
            butt.apparenceItem.apparenceType = apparenceType;
        }

        categoryName_Text.text = "" + MemberCreator.Instance.apparenceType_Names[(int)apparenceType];
    }

    public void CenterOnElement(Transform target)
    {
        Vector2 p =
            (Vector2)scrollView_RectTransform.InverseTransformPoint(content_RectTransform.position)
            - (Vector2)scrollView_RectTransform.InverseTransformPoint(target.position);

        p = p + scrollView_RectTransform.rect.size / 2f;

        content_RectTransform.DOAnchorPos(p, 0.5f);
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
