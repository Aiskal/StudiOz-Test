using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject CurrentObject { get; set; }

    [HideInInspector]
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsEmpty()
    {
        return CurrentObject == null;
    }
    public IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            if (!obj.GetComponent<InventoryObject>().isInInventory) Destroy(obj);
            if (CurrentObject == obj)
                CurrentObject = null;
        }
    }
}
