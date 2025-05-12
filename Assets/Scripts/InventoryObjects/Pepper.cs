using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pepper : InventoryObject
{
    private Coroutine multipliyerCoroutine;

    public override void UsePower()
    {
        base.UsePower();

        if (multipliyerCoroutine != null)
        {
            GM.Instance.StopCoroutine(multipliyerCoroutine);
        }
        multipliyerCoroutine = GM.Instance.StartCoroutine(ChangeScoreMult());
    }

    private IEnumerator ChangeScoreMult()
    {
        GM.Instance.player.currentScoreMultipliyer = 2;
        yield return new WaitForSeconds(10f);
        GM.Instance.player.currentScoreMultipliyer = 1;

        multipliyerCoroutine = null;
    }
}