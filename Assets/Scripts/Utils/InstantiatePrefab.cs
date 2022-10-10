using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    [Header("Referencies")]
    public GameObject prefab;

    [Header("Configuration")]
    public Transform parentTransform;
    public Transform point;
    public float livingTime;
    public float waitingTimeToInstantiate;

    public void Instantiate()
    {
        StartCoroutine(nameof(InstantiateCoroutine));
    }

    public IEnumerator InstantiateCoroutine()
    {
        if (waitingTimeToInstantiate > 0)
            yield return new WaitForSeconds(waitingTimeToInstantiate);
        else
            yield return null;

        GameObject instantiatedObject = Instantiate(prefab, point.position, Quaternion.identity) as GameObject;

        if (parentTransform != null)
        {
            instantiatedObject.transform.SetParent(parentTransform);
        }

        if (livingTime > 0f)
        {
            Destroy(instantiatedObject, livingTime);
        }
    }
}
