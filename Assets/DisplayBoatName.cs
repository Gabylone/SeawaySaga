using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayBoatName : MonoBehaviour, IPointerClickHandler
{
    public static DisplayBoatName Instance;

    public InputField InputField;

    public float bounceAmount = 1.03f;

    public RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetRandomBoatName();

        InputField.onEndEdit.AddListener(delegate { ChangeName(InputField); });
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetRandomBoatName()
    {
        string boat_name = CrewCreator.Instance.boatNames[Random.Range(0, CrewCreator.Instance.boatNames.Length)];
        string boat_adjective = CrewCreator.Instance.boatAdjectives[Random.Range(0, CrewCreator.Instance.boatAdjectives.Length)];
        string boat_fulName = "The " + boat_adjective + " " + boat_name;

        Boats.Instance.playerBoatInfo.Name = boat_fulName;
        InputField.text = boat_fulName;

        Tween.Bounce(rectTransform, 0.2f, bounceAmount);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Tween.Bounce(rectTransform, 0.2f, bounceAmount);

        SoundManager.Instance.PlaySound("click_med 03");

        InputField.interactable = true;
        InputField.ActivateInputField();
    }

    public void ChangeName(InputField inputField)
    {
        Tween.Bounce(rectTransform, 0.2f, bounceAmount);

        if (InputField.text.StartsWith("The "))
        {

        }
        else
        {
            InputField.text = "The " + InputField.text;
        }

        Boats.Instance.playerBoatInfo.Name = InputField.text;
        SoundManager.Instance.PlaySound("click_med 03");
    }
}
