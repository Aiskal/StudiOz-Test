using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pear : InventoryObject
{
    public override void UsePower()
    {
        base.UsePower();

        for(int i = 0; i < 5; i++)
        {
            GM.Instance.SpawnRandomScoreObjects();
        }
    }
}