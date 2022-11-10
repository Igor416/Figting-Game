using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] EnemyPrefabs;

    public int Distance { get; set; }
    public bool Active { get; set; } = true;

    void Start()
    {
        if (CalcPercent(Distance) > Random.Range(0, 100)) {
            /*
             * Do when have more heroes than one
             * Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)]);
            */
            Instantiate(EnemyPrefabs[0], transform);
        }
    }

    private int CalcPercent(int a) {
        if (Active) {
            return a > 4 ? 99 : 3 * a * a + 5 * a; // special func, that yiels [(1, 8), (2, 22), (3, 42), (4, 68), (5, 100)]
        }
        return 0;
    }
}
