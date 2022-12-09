using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatsMenu : MonoBehaviour
{
    LevelBuilder Script;
    bool CheatsEnabled = false;

    private void Start()
    {
        Script = GetComponent<LevelBuilder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            CheatsEnabled = !CheatsEnabled;
            Script.Alert("Cheats: " + CheatsEnabled);
        }

        if (CheatsEnabled)
        {
            if (Input.GetKeyDown(KeyCode.F1)) { 
                if (Script.SaveGame.intData.difficulty == 0)
                { Script.SaveGame.intData.difficulty = Script.BombDifficulty; Script.Alert("BombSpawn: true"); }
                else
                { Script.SaveGame.intData.difficulty = 0; Script.Alert("BombSpawn: false"); }
            }
            if (Input.GetKeyDown(KeyCode.F2)) { Script.EnableBombSight = !Script.EnableBombSight; Script.Alert("Bombsight: " + Script.EnableBombSight); }
            if (Input.GetKey(KeyCode.F3)) { Script.SaveGame.intData.totalCoins++; Script.Alert("Coin"); }
            if (Input.GetKeyDown(KeyCode.F4)) {
                if (Script.SaveGame.intData.itemTotal < 3)
                {
                    Script.SaveGame.collectedItems.Add(Script.Items[Script.SaveGame.intData.itemTotal]);
                    Script.Alert("Item");
                    Script.Alert("You collected an item!");

                    if (Script.SaveGame.intData.itemTotal == 1)
                    {
                        Script.SaveGame.enableSight = true;
                    }
                    else if (Script.SaveGame.intData.itemTotal == 2)
                    {
                        Script.SaveGame.hasCompass = true;
                    }
                    else if (Script.SaveGame.intData.itemTotal == 3)
                    {
                        Script.SaveGame.hasDetonator = true;
                    }
                }
            }
        }

    }
}
