using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameButton : MonoBehaviour
{
    public LandPlacement LP;
    public string SaveGame = "";

    public void DoIt()
    {
        LP.LoadSavegame(SaveGame);
        //LP.LoadGame(SaveGame);
    }
}
