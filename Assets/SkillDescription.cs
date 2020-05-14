using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class SkillDescription : MonoBehaviour
{
    public static SkillDescription Instance;

    public GameObject group;

    public Text uiText_Title;
    public Text uiText_Description;

    public CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        HideDelay();
    }

    public void Show( Skill skill )
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f , 0.3f);

        group.SetActive(true);

        uiText_Title.text = skill.skillName;
        uiText_Description.text = skill.description;
    }

    public void Hide()
    {
        canvasGroup.DOFade( 0f , 0.3f );

        Invoke("HideDelay" , 0.3f);
    }

    void HideDelay()
    {
        group.SetActive(false);
    }
}
