using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFastTravelInfo : Displayable
{
    public static DisplayFastTravelInfo Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Text travel_Text;
    public Text crew_Text;
    public Text food_Text;

    public override void Start()
    {
        base.Start();
    }

    public void Display ( int travel, int crewHunger , int food )
    {
        Show();

        travel_Text.text = "-" + travel;
        crew_Text.text = "" + crewHunger;
        food_Text.text = "" + food;
    }

    public void SetRed()
    {
        travel_Text.color = Color.red;
        crew_Text.color = Color.red;
        food_Text.color = Color.red;
    }

    public void SetBlack()
    {
        travel_Text.color = Color.black;
        crew_Text.color = Color.black;
        food_Text.color = Color.black;
    }

}
