using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour {
    public List<GameObject> items = new List<GameObject>();
    public GameObject GroundPrefab;

    private const int distance = 6;

    private IEnumerator SpawnGround(int distance) {
        Vector3 pos;

        for (int i = -distance; i < distance; i++) {
            pos = new Vector3(i, 0, -distance);
            InstantiateGroundPrefab(pos, distance);
        }
        for (int i = distance; i > -distance; i--) {
            pos = new Vector3(-distance, 0, i);
            InstantiateGroundPrefab(pos, distance);
        }
        for (int i = distance; i > -distance; i--) {
            pos = new Vector3(i, 0, distance);
            InstantiateGroundPrefab(pos, distance);
        }
        for (int i = -distance; i < distance; i++) {
            pos = new Vector3(distance, 0, i);
            InstantiateGroundPrefab(pos, distance);
        }

        yield return new WaitForSeconds(1f);
    }

    void Start() {
        var ground = Instantiate(GroundPrefab, transform).GetComponent<EnemySpawner>();
        ground.Active = false;

        for (int i = 1; i < distance; i++) {
            StartCoroutine(SpawnGround(i));
        }
    }

    private void InstantiateGroundPrefab(Vector3 pos, int distance) {
        var ground = Instantiate(GroundPrefab, pos * 10, Quaternion.Euler(0, 0, 0), transform).GetComponent<EnemySpawner>();
        if (distance == GroundGenerator.distance - 1) {
            ground.Active = false;
            if (pos.x == 0 || pos.z == 0 || Mathf.Abs(pos.x) == Mathf.Abs(pos.z)) {
                var item = items[Random.Range(0, items.Count)];
                ground.GetComponent<ItemSpawner>().Item = item;
                items.Remove(item);
            }
        } else {
            ground.Distance = distance;
        }
    }
}
