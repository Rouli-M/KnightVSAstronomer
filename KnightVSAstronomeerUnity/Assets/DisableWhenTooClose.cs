using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenTooClose : MonoBehaviour
{
    public List<Transform> disableIfTooClose;

    // Update is called once per frame
    void Update()
    {
        foreach(Transform t in disableIfTooClose)
        {

            t.gameObject.SetActive((t.transform.position - transform.position).magnitude > 30);
        }
    }
}
