using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPurchase : MonoBehaviour {

    /*public static DisplayPurchase Instance;

    public GameObject group;

    public Transform itemAnchor;

    ApparenceItem apparenceItem;

    Transform targetTransform;
    Transform initParent;

    public Button purchaseButton;

    public Text pearlPrice_Text;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();
    }

    public void Display ( ApparenceItem _item , Transform _targetTransform)
    {
        Show();

        this.apparenceItem = _item;

        GameObject g = Instantiate(_targetTransform.gameObject, itemAnchor) as GameObject;
        targetTransform = g.transform;
        targetTransform.localPosition = Vector3.zero;

        targetTransform.GetComponent<RectTransform>().sizeDelta = itemAnchor.GetComponent<RectTransform>().sizeDelta;

        Tween.Bounce(group.transform);

    }

    void Show()
    {
        group.SetActive(true);
    }

    void Hide()
    {
        group.SetActive(false);
    }

    public void Buy()
    {
        apparenceItem.locked = false;

        PlayerInfo.Instance.AddApparenceItem(apparenceItem);

        CrewCreator.Instance.UpdateApparenceItems();

        targetTransform.GetComponent<Map>().UpdateUI();

        Close();

        PlayerInfo.Instance.Save();
    }

    public void Close()
    {
        Destroy(targetTransform.gameObject);

        Hide();
    }*/

}
