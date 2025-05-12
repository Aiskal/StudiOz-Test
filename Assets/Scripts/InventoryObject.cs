using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InventoryObject : MonoBehaviour
{
    public Sprite sprite;

    [HideInInspector]
    public bool isInInventory = false;

    public virtual void UsePower()
    {
        Debug.Log("Using Power!");
    }
}
