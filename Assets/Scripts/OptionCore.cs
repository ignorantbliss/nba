using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionCore : MonoBehaviour
{
    public static OptionCore Instance;

    public bool Cheat_FlightMode;
    public UnityEngine.UI.Image Cheat_FlightMode_Button;

    void Start()
    {
        Instance = this;
    }

    public void ToggleFlight()
    {
        Cheat_FlightMode = !Cheat_FlightMode;
        if (Cheat_FlightMode == false)
        {
            Cheat_FlightMode_Button.color = Color.red;
        }
        else
        {
            Cheat_FlightMode_Button.color = Color.green;
        }
    }

    public int LevelMusic = 1;
    public string WorldFloor = "Water";
    public TextureChanges _WorldFloorChanger;

    public void ChangeOptionInt(string Name,int Value)
    {
        switch (Name)
        {
            case "LevelMusic":
                LevelMusic = Value;
                break;
            case "WorldFloor":
                WorldFloor = Value.ToString();
                _WorldFloorChanger.SetMaterials(Value);
                break;
        }
    }

    public void ChangeOptionString(string Name, string Value)
    {
        switch(Name)
        {
            case "LevelMusic":
                LevelMusic = int.Parse(Value);
                break;
            case "WorldFloor":
                WorldFloor = Value;                
                break;
        }
    }
}
