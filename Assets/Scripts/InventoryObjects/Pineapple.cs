using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pineapple : InventoryObject
{
    public override void UsePower()
    {
        base.UsePower();
        GM.Instance.remainingSeconds += 15;
    }
}
