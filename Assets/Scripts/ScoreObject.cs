using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    public int score;
    public Coroutine destroyCoroutine;

    public void StartDestroyCoroutine(float delay)
    {
        destroyCoroutine = StartCoroutine(DestroyAfterDelay(delay));
    }

    public IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (this != null && this.gameObject != null) Destroy(this.gameObject);
    }
}
