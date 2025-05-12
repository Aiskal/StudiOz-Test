using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapes : InventoryObject
{
    private Coroutine speedCoroutine;

    public override void UsePower()
    {
        base.UsePower();

        if (speedCoroutine != null)
        {
            GM.Instance.StopCoroutine(speedCoroutine);
        }
        
        speedCoroutine = GM.Instance.StartCoroutine(ChangeSpeed());
    }

    private IEnumerator ChangeSpeed()
    {
        GM.Instance.player.currentMoveSpeed *= 1.5f;
        yield return new WaitForSeconds(5f);
        GM.Instance.player.currentMoveSpeed = GM.Instance.player.moveSpeed;
        speedCoroutine = null;
    }
}
