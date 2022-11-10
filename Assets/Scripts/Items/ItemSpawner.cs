using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject Item {
        set {
            (int x, int z) = (Random.Range(-3, 4), Random.Range(-3, 4));
            Instantiate(value, transform.position + new Vector3(x, 0, z), Quaternion.Euler(90, 0, 0), transform);
        }
    }
}
