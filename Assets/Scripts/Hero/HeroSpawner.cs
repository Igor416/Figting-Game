using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSpawner : MonoBehaviour {

    public GameObject HeroPrefab;
    public GameObject[] InitialItems;

    public Transform mainCamera;
    private GameObject Hero;
    private const float maxCameraOffset = 3f;

    void Start() {
        Hero = Instantiate(HeroPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), transform);
        var inventory = Hero.GetComponent<Inventory>();
        inventory.Add(InitialItems, true);
    }

    void FixedUpdate() {
        float downAngle = Hero.GetComponent<HeroMovementController>().Falling ? 45 : 30;
        mainCamera.rotation = Quaternion.Euler(downAngle, 0, 0);
        Vector3 offset = new Vector3(0, maxCameraOffset, -maxCameraOffset);
        mainCamera.position = Hero.transform.position + offset;
    }
}
