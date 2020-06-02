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

            butt.image.transform.localPosition += (Vector3)buttonDecal;
            butt.image.transform.localScale = Vector3.one * scaleMult;
            
        }

        categoryName_Text.text = "" + apparenceType.ToString();

        //initScale = rectTransform.sizeDelta;

        //initCellScale = gridLayout.cellSize.x;
        //gridLayout.cellSize = Vector2.one * initCellScale;
    }

    /*public void OnPointerDown()
    {
        ShowItems();
    }

    public void OnPointerUp()
    {
        HideItems();
    }*/

    private void Update()
    {
        /*if (showing)
        {
            rectTransform.sizeDelta = Vector2.MoveTowards(rectTransform.sizeDelta, contentFitter.sizeDelta, speed * Time.deltaTime);
            gridLayout.cellSize = Vector2.MoveTowards( gridLayout.cellSize , Vector2.one * targetCellScale , speed * Time.deltaTime );
        }*/
    }

    void ShowItems()
    {
        showing = true;

        //HOTween.To(rectTransform, dur, "sizeDelta", contentFitter.sizeDelta);
        //HOTween.To(gridLayout, dur, "cellSize", Vector2.one * targetCellScale);

        /*foreach (var item in GetComponentsInChildren<MemberCreationButton_Apparence>())
        {
            item.GetComponent<Image>().raycastTarget = true;
        }*/

        //transform.SetAsLastSibling();
    }

    void HideItems()
    {
        showing = false;

        /*rectTransform.DOSizeDelta(initScale, dur);
        gridLayout.cellSize = Vector2.one * initCellScale;

        if (MemberCreationButton_Apparence.lastSelected != null)
        {
            MemberCreationButton_Apparence.lastSelected.OnPointerUp();
            MemberCreationButton_Apparence.lastSelected.transform.SetAsFirstSibling();
        }

        foreach (var item in GetComponentsInChildren<MemberCreationButton_Apparence>())
        {
            item.GetComponent<Image>().raycastTarget = false;
        }*/
    }
}
